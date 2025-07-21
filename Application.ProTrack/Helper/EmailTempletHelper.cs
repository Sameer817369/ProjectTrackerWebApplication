using System;

namespace Application.ProTrack.Helper
{
    public static class EmailTempletHelper
    {
        public static string WrapInStandardTemplate(string recipientName, string messageBodyHtml)
        {
            return $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1'>
                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
                        <style>
                            body {{
                                background-color: #f8f9fa;
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            }}
                            .email-container {{
                                max-width: 700px;
                                margin: auto;
                                padding: 30px;
                            }}
                            .email-header {{
                                background-color: #0d6efd;
                                color: #ffffff;
                                padding: 20px;
                                border-top-left-radius: 0.5rem;
                                border-top-right-radius: 0.5rem;
                            }}
                            .email-footer {{
                                font-size: 0.9rem;
                                color: #6c757d;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='card shadow-sm'>
                                <div class='email-header'>
                                    <h2 class='mb-0'>Hello {recipientName},</h2>
                                </div>
                                <div class='card-body'>
                                    {messageBodyHtml}
                                    <hr class='my-4'>
                                    <p class='email-footer'>
                                        Best regards,<br/>
                                        <strong>Project Coordination Team</strong>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>"
            ;
        }
    }
}