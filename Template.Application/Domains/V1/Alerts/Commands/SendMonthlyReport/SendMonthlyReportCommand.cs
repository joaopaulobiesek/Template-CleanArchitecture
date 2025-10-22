namespace Template.Application.Domains.V1.Alerts.Commands.SendMonthlyReport;

/// <summary>
/// Command para enviar relatório mensal para todos os usuários ativos
/// Executado dia 1 de cada mês às 9h pelo Hangfire job
/// Processo:
/// 1. Busca todos os usuários com FinancialAccount ativa
/// 2. Busca Budget do mês anterior
/// 3. Busca todos os EnvelopeBudgets do mês anterior
/// 4. Calcula performance (envelopes no limite vs estourados)
/// 5. Compara com mês anterior (se houver)
/// 6. Monta relatório completo formatado
/// 7. Envia via WhatsApp
/// </summary>
public class SendMonthlyReportCommand
{
    // Sem propriedades - job executa para todos os usuários
}
