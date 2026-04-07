import "./globals.css";
import { Providers } from "@/app/providers";

export const metadata = {
  title: "Experiment Analytics Dashboard",
  description: "Dashboard analítico para avaliação de experimentos MCP e LLMs."
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="pt-BR">
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
