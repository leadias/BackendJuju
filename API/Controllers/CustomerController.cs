using Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomerEntity = DataAccess.Data.Customer;
using PostEntity = DataAccess.Data.Post;

namespace API.Controllers.Customer
{
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private BaseService<CustomerEntity> CustomerService;
        private BaseService<PostEntity> PostService;

        public CustomerController(BaseService<CustomerEntity> customerService, BaseService<PostEntity> postService)
        {
            CustomerService = customerService;
            PostService = postService;
        }


        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                IQueryable<CustomerEntity> list  = CustomerService.GetAll();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = list});

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }

        }


        [HttpPost()]
        public IActionResult Create([FromBodyAttribute] CustomerEntity entity)
        {

            try
            {
                var customers = CustomerService.GetAll();

                foreach (var item in customers)
                {
                    if (item.Name == entity.Name)
                    {
                        return BadRequest("you already mentioned the name.");
                    }
                }

                CreateCustomer(entity);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Customer create" });

            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }


        }

        private CustomerEntity CreateCustomer(CustomerEntity entity)
        {
            return CustomerService.Create(entity);
        }

        [HttpPut()]
        public CustomerEntity Update(CustomerEntity entity)
        {
            return CustomerService.Update(entity.CustomerId, entity, out bool changed);
        }

        [HttpDelete()]
        public IActionResult Delete([FromBodyAttribute] CustomerEntity entity)
        {
            try
            {
                var post = PostService.GetAll();
                var postUser = post.Where(x => x.CustomerId == entity.CustomerId).ToList();

                foreach (var item in postUser)
                {
                    PostService.Delete(item);
                }

                CustomerService.Delete(entity);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Customer delete" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message });
            }

        }
    }
}
