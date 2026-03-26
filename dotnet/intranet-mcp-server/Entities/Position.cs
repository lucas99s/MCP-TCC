namespace intranet_mcp_server.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty; // (Junior, Mid, Senior, Especialista, Gestor)
    }
}
