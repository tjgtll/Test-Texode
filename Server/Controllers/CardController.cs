using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/card")]
    public class CardController : Controller
    {
        private readonly ICardRepo _repo;
        private IHostingEnvironment _hostingEnvironment;
        private string path;

        public CardController(ICardRepo cardRepo, IHostingEnvironment environment)
        {
            _repo = cardRepo;
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            path = Path.Combine(_hostingEnvironment.WebRootPath, "image/");
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get()
        {
            path = Path.Combine(_hostingEnvironment.WebRootPath);
            if ((!Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }
            var result = _repo.All();
            return Ok(result);
        }

        [HttpGet]
        [Route("get/c")]
        public async Task<IActionResult> Get(string name)
        {
            if (name == null)
            {
                return NotFound();
            }
            var result = _repo.Get(name);
            return Ok(result);
        }

        [HttpGet]
        [Route("get/image")]
        public async Task<IActionResult> GetImage(string pathImage)
        {
            if (pathImage == null)
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead($"wwwroot\\image\\{pathImage}");
            return File(image, "image/png");
        }

        [HttpPut]
        [Route("put")]
        public async Task<IActionResult> Update(string oldName, [FromForm] Card model)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if ((!Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = $"{Guid.NewGuid().ToString()}.png";
                using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }
                model.Image = filename;

                _repo.Update(oldName, model);

                resp.Add("status ", "success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(resp);
        }

        [HttpPut]
        [Route("put/n")]
        public async Task<IActionResult> Update(string oldName, string newName)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();
            if (oldName is null)
            {
                return BadRequest(oldName);
            }
            if (newName is null)
            {
                return BadRequest(newName);
            }
            try
            {
                if ((!Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }

                _repo.Update(oldName, newName);
                resp.Add("status ", "success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(resp);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> delete(string name)
        {
            if (name is null)
            {
                return BadRequest();
            }

            _repo.Delete(name);

            return Ok();
        }

        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> Post([FromForm] Card model)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if ((!Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = model.ProfileImage.FileName;
                using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }
                model.Image = filename;
                _repo.Create(model);
                resp.Add("status ", "success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(resp);
        }
    }
}
