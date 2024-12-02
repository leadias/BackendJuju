using Business;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PostEntity = DataAccess.Data.Post;
using CustomerEntity = DataAccess.Data.Customer;
using Microsoft.AspNetCore.Http;
using System;
using DataAccess.Data;
using System.Collections.Generic;
using static Dapper.SqlMapper;

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

        //Creacion API que permita crear N cantidad de Post al mismo tiempo.

        [HttpPost("postDtos")]

        public IActionResult CreateDtos([FromBodyAttribute] List<PostEntity> postDtos)
        {

            try
            {
                if (postDtos == null || !postDtos.Any())
                {
                    return BadRequest("No valid posts will be provided.");
                }


                var posts = postDtos.Select(postDto => new PostEntity
                {
                    PostId = postDto.PostId,
                    Title = postDto.Title,
                    Body = postDto.Body,
                    Type = postDto.Type,
                    Category = postDto.Category,
                    CustomerId = postDto.CustomerId
                }).ToList();

                foreach (var item in posts)
                {
                    PostService.Create(item);
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Post create" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }



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
        public IActionResult Update([FromBodyAttribute] PostEntity entity)
        {
            try
            {
                PostService.Update(entity.PostId, entity, out bool changed);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Post update" }); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }

        }

        [HttpDelete()]
        public IActionResult Delete([FromBodyAttribute] PostEntity entity)
        {
            try
            {
                PostService.Delete(entity);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Post delete" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }

        }


    }
}
