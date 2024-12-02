using Business;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PostEntity = DataAccess.Data.Post;
using CustomerEntity = DataAccess.Data.Customer;
using Microsoft.AspNetCore.Http;
using System;
using DataAccess.Data;

namespace API.Controllers.Post
{
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private BaseService<PostEntity> PostService;
        private BaseService<CustomerEntity> CustomerService;
        public PostController(BaseService<PostEntity> postService, BaseService<CustomerEntity> customerService)
        {
            PostService = postService;
            CustomerService = customerService;
        }

        [HttpGet()]
        public IQueryable<PostEntity> GetAll()
        {
            return PostService.GetAll();
        }

        [HttpPost()]
        public IActionResult Create([FromBodyAttribute]  PostEntity entity)
        {
            try
            {
                var user = CustomerService.GetAsync(entity.CustomerId);
                if (user == null)
                {
                    return BadRequest("Associated user not found.");
                }
                if (!string.IsNullOrEmpty(entity.Body) && entity.Body.Length > 20)
                {
                    // Cortar el cuerpo a 97 caracteres y agregar "..."
                    entity.Body = entity.Body.Length > 97 ? entity.Body.Substring(0, 97) + "..." : entity.Body;
                }

                switch (entity.Type)
                {
                    case 1:
                        entity.Category = "Farándula";
                        break;
                    case 2:
                        entity.Category = "Política";
                        break;
                    case 3:
                        entity.Category = "Futbol";
                        break;
                }

                PostService.Create(entity);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Post create" });
            }
            catch(Exception ex) 
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }

        }

        [HttpPut()]
        public PostEntity Update([FromBodyAttribute] PostEntity entity)
        {
            return PostService.Update(entity.PostId,entity, out bool changed);
        }

        [HttpDelete()]
        public PostEntity Delete([FromBodyAttribute] PostEntity entity)
        {
            return PostService.Delete(entity);
        }


    }
}
