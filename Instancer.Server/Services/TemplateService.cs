using Instancer.Server.Models;

namespace Instancer.Server.Services
{
    public class TemplateService
    {
        private readonly string _templateRoot = Path.Combine(Directory.GetCurrentDirectory(), "templates");

        public IEnumerable<TemplateInfo> GetTemplates()
        {
            foreach (var dir in Directory.GetDirectories(_templateRoot))
            {
                var metaFile = Path.Combine(dir, "meta.json");
                if (File.Exists(metaFile))
                {
                    var json = File.ReadAllText(metaFile);
                    var info = System.Text.Json.JsonSerializer.Deserialize<TemplateInfo>(json);
                    if (info != null)
                    {
                        info.Id = Path.GetFileName(dir); // injecte l'id depuis le nom du dossier
                        yield return info;
                    }
                }
            }
        }
    }
}
