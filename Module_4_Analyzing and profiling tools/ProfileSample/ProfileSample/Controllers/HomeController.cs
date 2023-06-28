using ProfileSample.DAL;
using ProfileSample.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            var images = new Queue<ImgSource>();
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
                        images.Enqueue(entity);
                    }
                }));
            }
            await Task.WhenAll(tasks);

            //We shouldn't use one DbContext instance in multiple threads
            using (var context = new ProfileSampleEntities())
            {
                context.ImgSources.AddRange(images);
                await context.SaveChangesAsync();
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