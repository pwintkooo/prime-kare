using System.Net;

namespace PrimeKare.Api.Templates;

public static class EmailLayoutTemplate
{
    public static string Build(
        string frontendUrl,
        string content)
    {
        var safeFrontendUrl =
            WebUtility.HtmlEncode(frontendUrl.TrimEnd('/'));

        var logoUrl =
            $"{safeFrontendUrl}/images/branding/primekare-dark-logo.png";

        return $"""
            <!DOCTYPE html>
            <html>
            <body style="
                margin: 0;
                padding: 0;
                background-color: #f8fafc;
                font-family: Arial, sans-serif;
                color: #0f172a;
            ">
                <div style="
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 32px 16px;
                ">
                    <div style="
                        background-color: #ffffff;
                        border: 1px solid #e2e8f0;
                        border-radius: 12px;
                        overflow: hidden;
                    ">
                        <!-- Header -->
                        <div style="
                            background-color: #020617;
                            padding: 24px;
                            text-align: center;
                        ">
                            <a
                                href="{safeFrontendUrl}"
                                target="_blank"
                                style="text-decoration: none;"
                            >
                                <img
                                    src="{logoUrl}"
                                    alt="PrimeKare"
                                    width="160"
                                    style="
                                        display: block;
                                        margin: 0 auto;
                                        max-width: 160px;
                                        height: auto;
                                    "
                                />
                            </a>
                        </div>

                        <!-- Content -->
                        <div style="padding: 32px 24px;">
                            {content}
                        </div>

                        <!-- Footer -->
                        <div style="
                            padding: 20px 24px;
                            background-color: #f8fafc;
                            border-top: 1px solid #e2e8f0;
                            text-align: center;
                            font-size: 12px;
                            color: #64748b;
                        ">
                            <p style="margin: 0;">
                                © {DateTime.UtcNow.Year} PrimeKare.
                                All rights reserved.
                            </p>
                        </div>
                    </div>
                </div>
            </body>
            </html>
            """;
    }
}