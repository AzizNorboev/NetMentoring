using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            using (var context = new ProfileSampleEntities())
            {
                var model = await context.ImgSources
                                         .AsNoTracking()
                                         .Take(20)
                                         .Select(x => new ImageModel
                                         {
                                             Name = x.Name,
                                             Data = x.Data
                                         })
                                         .ToListAsync();

                return View(model);
            }
        }

        public async Task<ActionResult> Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                var tasks = new List<Task>();
                foreach (var file in files)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using (var stream = new FileStream(file, FileMode.Open))
                        {
                            byte[] buff = new byte[stream.Length];

                            await stream.ReadAsync(buff, 0, (int)stream.Length);

                            var entity = new ImgSource()
                            {
                                Name = Path.GetFileName(file),
                                Data = buff,
                            };

                            context.ImgSources.Add(entity);
                            await context.SaveChangesAsync();
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}