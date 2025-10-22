namespace Template.Application.Domains.V1.Alerts.Commands.SendWeeklyReport;

/// <summary>
/// Command para enviar relatório semanal para todos os usuários ativos
/// Executado todo domingo às 9h pelo Hangfire job
/// Processo:
/// 1. Busca todos os usuários com FinancialAccount ativa
/// 2. Para cada usuário, busca transações dos últimos 7 dias
/// 3. Calcula totais por envelope
/// 4. Monta relatório formatado
/// 5. Envia via WhatsApp
/// </summary>
public class SendWeeklyReportCommand
{
    // Sem propriedades - job executa para todos os usuários
}
