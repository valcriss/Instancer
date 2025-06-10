namespace Instancer.Server.Dtos
{
    public class CreateStackRequest
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public Dictionary<string, string>? Variables { get; set; }
    }
}
