using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

public class MailSettings
{
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}

public class MailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendAppointmentConfirmationAsync(
        string toEmail,
        string patientName,
        DateTime appointmentDate,
        TimeSpan startTime,
        TimeSpan endTime,
        string doctorName,
        string location)
    {
        var subject = "Confirmación de Cita - Clínica Dental Alegría";
        var hora = $"{startTime.Hours:D2}:{startTime.Minutes:D2}";
        var htmlBody = $@"
            <h3>Hola {patientName},</h3>
            <p>Te confirmamos tu cita en la <strong>Clínica Dental Alegría</strong>.</p>
            <p><strong>Fecha:</strong> {appointmentDate:dd/MM/yyyy}</p>
            <p><strong>Hora:</strong> {hora} hrs (hora local de Guatemala)</p>
            <p><strong>Odontólogo(a):</strong> {doctorName}</p>
            <p><strong>Sede:</strong> {location}</p>
            <p>Adjuntamos una invitación (.ics) para añadirla a tu calendario.</p>
            <p>¡Te esperamos!</p>";

        await SendEmailWithCalendarEventAsync(toEmail, patientName, subject, htmlBody, appointmentDate, startTime, endTime, doctorName, location);
    }

    public async Task SendRescheduleNotificationAsync(
        string toEmail,
        string patientName,
        DateTime newAppointmentDate,
        TimeSpan newStartTime,
        TimeSpan newEndTime,
        string doctorName,
        string location)
    {
        var subject = "Aviso de Reagendamiento de Cita - Clínica Dental Alegría";
        var hora = $"{newStartTime.Hours:D2}:{newStartTime.Minutes:D2}";
        var htmlBody = $@"
            <h3>Hola {patientName},</h3>
            <p>Te informamos que tu cita en la <strong>Clínica Dental Alegría</strong> ha sido <strong>REAGENDADA</strong>.</p>
            <p><strong>Nueva Fecha:</strong> {newAppointmentDate:dd/MM/yyyy}</p>
            <p><strong>Nueva Hora:</strong> {hora} hrs (hora local de Guatemala)</p>
            <p><strong>Odontólogo(a):</strong> {doctorName}</p>
            <p><strong>Sede:</strong> {location}</p>
            <p>Adjuntamos la invitación actualizada (.ics) para tu calendario.</p>";

        await SendEmailWithCalendarEventAsync(toEmail, patientName, subject, htmlBody, newAppointmentDate, newStartTime, newEndTime, doctorName, location);
    }

    public async Task SendUpdateNotificationAsync(
        string toEmail,
        string patientName,
        DateTime appointmentDate,
        TimeSpan startTime,
        TimeSpan endTime,
        string doctorName,
        string location)
    {
        var subject = "Aviso de Actualización de Cita - Clínica Dental Alegría";
        var hora = $"{startTime.Hours:D2}:{startTime.Minutes:D2}";
        var htmlBody = $@"
            <h3>Hola {patientName},</h3>
            <p>Te informamos que los detalles de tu cita en la <strong>Clínica Dental Alegría</strong> han sido <strong>ACTUALIZADOS</strong>.</p>
            <p>Los datos más recientes son:</p>
            <p><strong>Fecha:</strong> {appointmentDate:dd/MM/yyyy}</p>
            <p><strong>Hora:</strong> {hora} hrs (hora local de Guatemala)</p>
            <p><strong>Odontólogo(a):</strong> {doctorName}</p>
            <p><strong>Sede:</strong> {location}</p>
            <p>Hemos actualizado la invitación en tu calendario.</p>";

        await SendEmailWithCalendarEventAsync(toEmail, patientName, subject, htmlBody, appointmentDate, startTime, endTime, doctorName, location);
    }

    private async Task SendEmailWithCalendarEventAsync(
        string toEmail, string patientName, string subject, string htmlBody,
        DateTime appointmentDate, TimeSpan startTime, TimeSpan endTime, string doctorName, string location)
    {
        var calendar = new Calendar { Method = "REQUEST", ProductId = "-//Clinica Dental Alegría//Citas//ES" };
        var tz = new VTimeZone("America/Guatemala");
        calendar.AddTimeZone(tz);

        var startLocal = new CalDateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, startTime.Hours, startTime.Minutes, 0, "America/Guatemala");
        var endLocal = new CalDateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, endTime.Hours, endTime.Minutes, 0, "America/Guatemala");

        var evt = new CalendarEvent
        {
            Summary = subject,
            Description = $"Cita con {doctorName} en {location}.",
            Location = location,
            Start = startLocal,
            End = endLocal,
            Uid = Guid.NewGuid().ToString(),
            Status = "CONFIRMED",
            Organizer = new Organizer($"MAILTO:{_mailSettings.Mail}") { CommonName = _mailSettings.DisplayName }
        };
        evt.Attendees.Add(new Attendee($"MAILTO:{toEmail}") { CommonName = patientName, Rsvp = true });
        evt.Alarms.Add(new Alarm { Action = AlarmAction.Display, Trigger = new Trigger(new Duration(-0, 0, -30, 0)), Description = "Recordatorio de cita odontológica" });
        calendar.Events.Add(evt);

        var serializer = new CalendarSerializer();
        var icsString = serializer.SerializeToString(calendar);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var htmlPart = new TextPart("html") { Text = htmlBody };
        var calendarPart = new TextPart("calendar") { Text = icsString };

        if (!calendarPart.ContentType.Parameters.Contains("method"))
            calendarPart.ContentType.Parameters.Add("method", "REQUEST");
        else
            calendarPart.ContentType.Parameters["method"] = "REQUEST";

        if (!calendarPart.ContentType.Parameters.Contains("charset"))
            calendarPart.ContentType.Parameters.Add("charset", "utf-8");
        else
            calendarPart.ContentType.Parameters["charset"] = "utf-8";

        if (!calendarPart.Headers.Contains("Content-Class"))
            calendarPart.Headers.Add("Content-Class", "urn:content-classes:calendarmessage");

        var multipart = new Multipart("alternative") { htmlPart, calendarPart };
        message.Body = multipart;

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    // --- 👇 MÉTODO NUEVO AÑADIDO ---
    public async Task SendCancellationNotificationAsync(
        string toEmail,
        string patientName,
        DateTime appointmentDate,
        TimeSpan startTime)
    {
        var subject = "Aviso de Cancelación de Cita - Clínica Dental Alegría";
        var hora = $"{startTime.Hours:D2}:{startTime.Minutes:D2}";

        var htmlBody = $@"
            <h3>Hola {patientName},</h3>
            <p>Te informamos que tu cita en la <strong>Clínica Dental Alegría</strong> ha sido <strong>CANCELADA</strong>.</p>
            <p>Detalles de la cita cancelada:</p>
            <ul>
                <li><strong>Fecha:</strong> {appointmentDate:dd/MM/yyyy}</li>
                <li><strong>Hora:</strong> {hora} hrs</li>
            </ul>
            <p>Si deseas programar una nueva cita, no dudes en contactarnos.</p>";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}