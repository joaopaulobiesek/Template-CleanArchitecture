namespace Template.Domain.Constants;

public static class Policies
{
    public const string CanList = nameof(CanList);                       // Permissão para listar itens
    public const string CanView = nameof(CanView);                       // Permissão para visualizar detalhes
    public const string CanCreate = nameof(CanCreate);                   // Permissão para criar novos itens
    public const string CanEdit = nameof(CanEdit);                       // Permissão para editar itens existentes
    public const string CanDelete = nameof(CanDelete);                   // Permissão para deletar itens
    public const string CanPurge = nameof(CanPurge);                     // Permissão para apagar itens permanentemente
    public const string CanArchive = nameof(CanArchive);                 // Permissão para arquivar itens
    public const string CanRestore = nameof(CanRestore);                 // Permissão para restaurar itens arquivados
    public const string CanApprove = nameof(CanApprove);                 // Permissão para aprovar itens
    public const string CanReject = nameof(CanReject);                   // Permissão para rejeitar itens
    public const string CanExport = nameof(CanExport);                   // Permissão para exportar dados
    public const string CanImport = nameof(CanImport);                   // Permissão para importar dados
    public const string CanManageSettings = nameof(CanManageSettings);   // Permissão para gerenciar configurações do sistema
    public const string CanManageUsers = nameof(CanManageUsers);         // Permissão para gerenciar usuários
    public const string CanAssignRoles = nameof(CanAssignRoles);         // Permissão para atribuir papéis/roles
    public const string CanAssignPolicies = nameof(CanAssignPolicies);   // Permissão para atribuir papéis/polices
    public const string CanViewReports = nameof(CanViewReports);         // Permissão para visualizar relatórios
    public const string CanGenerateReports = nameof(CanGenerateReports); // Permissão para gerar relatórios

    public static Dictionary<string, string> GetPolicies()
    {
        return new Dictionary<string, string>
        {
            { CanList, "Listar Itens" },                      // Permissão para listar itens
            { CanView, "Visualizar Detalhes" },               // Permissão para visualizar detalhes
            { CanCreate, "Criar Novos Itens" },               // Permissão para criar novos itens
            { CanEdit, "Editar Itens Existentes" },           // Permissão para editar itens existentes
            { CanDelete, "Deletar Itens" },                   // Permissão para deletar itens
            { CanPurge, "Apagar Permanentemente" },           // Permissão para apagar itens permanentemente
            { CanArchive, "Arquivar Itens" },                 // Permissão para arquivar itens
            { CanRestore, "Restaurar Itens Arquivados" },     // Permissão para restaurar itens arquivados
            { CanApprove, "Aprovar Itens" },                  // Permissão para aprovar itens
            { CanReject, "Rejeitar Itens" },                  // Permissão para rejeitar itens
            { CanExport, "Exportar Dados" },                  // Permissão para exportar dados
            { CanImport, "Importar Dados" },                  // Permissão para importar dados
            { CanManageSettings, "Gerenciar Configurações" }, // Permissão para gerenciar configurações do sistema
            { CanManageUsers, "Gerenciar Usuários" },         // Permissão para gerenciar usuários
            { CanAssignRoles, "Atribuir Funções" },           // Permissão para atribuir papéis/roles
            { CanAssignPolicies, "Atribuir Políticas" },       // Permissão para atribuir papéis/polices
            { CanViewReports, "Visualizar Relatórios" },      // Permissão para visualizar relatórios
            { CanGenerateReports, "Gerar Relatórios" }        // Permissão para gerar relatórios
        };
    }

    public static List<string> GetAllPolicies()
    {
        return new List<string>
        {
            CanList,
            CanView,
            CanCreate,
            CanEdit,
            CanDelete,
            CanPurge,
            CanArchive,
            CanRestore,
            CanApprove,
            CanReject,
            CanExport,
            CanImport,
            CanManageSettings,
            CanManageUsers,
            CanAssignRoles,
            CanAssignPolicies,
            CanViewReports,
            CanGenerateReports
        };
    }
}