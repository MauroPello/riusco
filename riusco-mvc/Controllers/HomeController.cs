using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using riusco_mvc.Models;
using riusco_mvc.ViewModels;

namespace riusco_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(string email, string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return RedirectToAction("Buy");

            if (Request.Method == "GET")
                return View(new GenericUserViewModel(email, outcome));

            switch (Request.Form["button"])
            {
                case "login":
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (requestMessage, certificate, chain, policyErrors) => true
                    };

                    using var httpClient = new HttpClient(handler);
                    using var request = new HttpRequestMessage(new HttpMethod("GET"),
                        $"{Request.Scheme}://{Request.Host}/Users/AuthenticateUser/");
                    var multipartContent = new MultipartFormDataContent {{new StringContent(Request.Form["password"]), "password"}, {new StringContent(Request.Form["email"]), "email"}};
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        HttpContext.Session.Remove("userId");
                        return RedirectToAction("Index", new GenericUserViewModel(Request.Form["email"], "error"));
                    }
            
                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var user = JsonSerializer.Deserialize<UserDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                        HttpContext.Session.SetInt32("userId", user.UserID);
                    }

                    return RedirectToAction("Profile");
                }
                case "register":
                {
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (requestMessage, certificate, chain, policyErrors) => true
                    };

                    using var httpClient = new HttpClient(handler);
                    using var request =
                        new HttpRequestMessage(new HttpMethod("POST"), $"{Request.Scheme}://{Request.Host}/Users/PostUser/");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(Request.Form["name"]), "name"},
                        {new StringContent(Request.Form["password"]), "password"},
                        {new StringContent(Request.Form["email"]), "email"},
                        {new StringContent(Request.Form["city"]), "city"}
                    };

                    var arr = Array.Empty<byte>();
                    if (Request.Form.Files.GetFile("image") != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Request.Form.Files.GetFile("image").CopyTo(memoryStream);
                            arr = memoryStream.ToArray();
                        }
                        multipartContent.Add(new ByteArrayContent(arr), "image", Request.Form.Files.GetFile("image").FileName);
                    }
                    else
                        multipartContent.Add(new ByteArrayContent(arr), "image", "no_image");
                        
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        HttpContext.Session.Remove("userId");
                        return RedirectToAction("Index", new GenericUserViewModel(Request.Form["email"], "error"));
                    }
                    
                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var newUser = JsonSerializer.Deserialize<UserDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                        HttpContext.Session.SetInt32("userId", newUser.UserID);
                    }
                    
                    return RedirectToAction("Profile");
                }
                default:
                    return View(new GenericUserViewModel(email, outcome));
            }
        }

        public IActionResult Products()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return RedirectToAction("Buy");
            
            return View();
        }
        
        [HttpGet] 
        [ActionName("Buy")]
        public IActionResult GetBuy()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return View(new BuyViewModel(userId.Value));
            
            return RedirectToAction("Index");
        }

        [HttpPost] 
        [ActionName("Buy")]
        public IActionResult PostBuy(int productId, int ownerId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request =
                    new HttpRequestMessage(new HttpMethod("POST"), $"{Request.Scheme}://{Request.Host}/Transactions/PostTransaction/"))
                {
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(productId.ToString()), "ProductID"},
                        {new StringContent(ownerId.ToString()), "OwnerID"},
                        {new StringContent(userId.ToString()), "BuyerID"},
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Buy");
                    }
                }
            }
            return RedirectToAction("Transactions");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
        
        [HttpGet] 
        [ActionName("LogIn")]
        public IActionResult GetLogIn(string email, string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return RedirectToAction("Profile");
            
            return View(new GenericUserViewModel(email, outcome));
        }

        [HttpPost] 
        [ActionName("LogIn")]
        public IActionResult PostLogIn(string email, string password)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("GET"),
                $"{Request.Scheme}://{Request.Host}/Users/AuthenticateUser/");
            var multipartContent = new MultipartFormDataContent {{new StringContent(password), "password"}, {new StringContent(email), "email"}};
            request.Content = multipartContent;

            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                HttpContext.Session.Remove("userId");
                return RedirectToAction("LogIn", new GenericUserViewModel(email, "error"));
            }
            
            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                var user = JsonSerializer.Deserialize<UserDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                HttpContext.Session.SetInt32("userId", user.UserID);
            }

            return RedirectToAction("Profile");
        }
        
        public IActionResult LogOut()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");
            
            HttpContext.Session.Remove("userId");
            Response.Headers.Add("REFRESH", "1;URL=/");
            return View();
        }
        
        [HttpGet] 
        [ActionName("Register")]
        public IActionResult GetRegister(string email, string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return RedirectToAction("Profile");
            
            return View(new GenericUserViewModel(email, outcome));
        }

        [HttpPost] 
        [ActionName("Register")]
        public IActionResult PostRegister([FromForm] User user)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using var httpClient = new HttpClient(handler);
            using var request =
                new HttpRequestMessage(new HttpMethod("POST"), $"{Request.Scheme}://{Request.Host}/Users/PostUser/");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(user.Name), "name"},
                {new StringContent(user.Password), "password"},
                {new StringContent(user.Email), "email"},
                {new StringContent(user.City), "city"}
            };

            var arr = Array.Empty<byte>();
            if (user.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    user.Image.CopyTo(memoryStream);
                    arr = memoryStream.ToArray();
                }
                multipartContent.Add(new ByteArrayContent(arr), "image", user.Image.FileName);
            }
            else
                multipartContent.Add(new ByteArrayContent(arr), "image", "no_image");
                
            request.Content = multipartContent;

            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                HttpContext.Session.Remove("userId");
                return RedirectToAction("Register", new GenericUserViewModel(user.Email, "error"));
            }
            
            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                var newUser = JsonSerializer.Deserialize<UserDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                HttpContext.Session.SetInt32("userId", newUser.UserID);
            }
            
            return RedirectToAction("Profile");
        }
        
        [HttpGet] 
        [ActionName("Profile")]
        public IActionResult GetProfile(string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");
            
            ProfileViewModel profileViewModel;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"),
                    $"{Request.Scheme}://{Request.Host}/Users/GetUser/{userId}"))
                {
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Error");
                    }

                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        profileViewModel = JsonSerializer.Deserialize<ProfileViewModel>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                    }
                }
            }

            profileViewModel.Outcome = outcome;
            return View(profileViewModel);
        }

        [HttpPost] 
        [ActionName("Profile")]
        public IActionResult PostProfile([FromForm] string button)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");
            
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };
            
            using var httpClient = new HttpClient(handler);
            switch (button)
            {
                case "update":
                {
                    using var request = new HttpRequestMessage(new HttpMethod("PUT"), $"{Request.Scheme}://{Request.Host}/Users/PutUser/{userId}");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(Request.Form["name"]), "name"},
                        {new StringContent(Request.Form["email"]), "email"},
                        {new StringContent(Request.Form["city"]), "city"},
                        {new StringContent(Request.Form["password"]), "password"},
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;
                        
                    var response = httpClient.Send(request);
                                            
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("Profile", new { outcome = "success" });
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Profile", new { outcome = "error" });
                    }
                }
                case "sign_out":
                {
                    return RedirectToAction("LogOut");
                }
                case "delete":
                {
                    using var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{Request.Scheme}://{Request.Host}/Users/DeleteUser/{userId}");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;
                    
                    var response = httpClient.Send(request);
                                        
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("LogOut");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Profile", new { outcome = "error" });
                    }
                }
                default:
                    return RedirectToAction("Profile", new { outcome = "error" });
            }
        }

        [HttpGet]
        [ActionName("DeleteProducts")]
        public IActionResult GetDeleteProducts(int value, string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return View(new GenericProductViewModel(value, outcome));

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("DeleteProducts")]
        public IActionResult PostDeleteProducts(int value)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };
            
            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("DELETE"),
                $"{Request.Scheme}://{Request.Host}/Products/DeleteProduct/{value}");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(_configuration["apiKey"]), "apiKey"}
            };
            request.Content = multipartContent;

            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
                return RedirectToAction("DeleteProducts", new GenericProductViewModel(value, "success"));
            }
            catch (Exception)
            {
                return RedirectToAction("DeleteProducts", new GenericProductViewModel(value, "error"));
            }
        }

        [HttpGet]
        [ActionName("UpdateProducts")]
        public IActionResult GetUpdateProducts(int value, string name, string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return View(new GenericProductViewModel(name, value, outcome));

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("UpdateProducts")]
        public IActionResult PostUpdateProducts(int value, [FromForm] Product product)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("PUT"),
                $"{Request.Scheme}://{Request.Host}/Products/PutProduct/{value}");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(product.Name ?? ""), "name"},
                {new StringContent(product.Description ?? ""), "description"},
                {new StringContent(_configuration["apiKey"]), "apiKey"}
            };

            var arr = Array.Empty<byte>();
            if (product.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    product.Image.CopyTo(memoryStream);
                    arr = memoryStream.ToArray();
                }

                multipartContent.Add(new ByteArrayContent(arr), "image", product.Image.FileName);
            }
            else
                multipartContent.Add(new ByteArrayContent(arr), "image", "no_image");

            request.Content = multipartContent;

            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
                return RedirectToAction("UpdateProducts", new GenericProductViewModel(product.Name, value, "success"));
            }
            catch (Exception)
            {
                return RedirectToAction("UpdateProducts", new GenericProductViewModel(product.Name, value, "error"));
            }
        }

        [HttpGet] [ActionName("Sell")]
        public IActionResult GetSell(string outcome)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId.HasValue)
                return View(new SellViewModel(userId.Value, outcome));

            return RedirectToAction("Index");
        }
        
        [HttpPost] [ActionName("Sell")]
        public IActionResult PostSell([FromForm] string button, [FromForm] Product product)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using var httpClient = new HttpClient(handler);
            switch (button)
            {
                case "insert":
                {
                    using var request =
                        new HttpRequestMessage(new HttpMethod("POST"), $"{Request.Scheme}://{Request.Host}/Products/PostProduct/");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(product.Name), "name"},
                        {new StringContent(product.Description), "description"},
                        {new StringContent(HttpContext.Session.GetInt32("userId").Value.ToString()), "userId"},
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };

                    var arr = Array.Empty<byte>();
                    if (product.Image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            product.Image.CopyTo(memoryStream);
                            arr = memoryStream.ToArray();
                        }
                        multipartContent.Add(new ByteArrayContent(arr), "image", product.Image.FileName);
                    }
                    else
                        multipartContent.Add(new ByteArrayContent(arr), "image", "no_image");
            
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Sell", new SellViewModel("error"));
                    }

                    break;
                }
                case "delete":
                {
                    using var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{Request.Scheme}://{Request.Host}/Products/DeleteProduct/{Request.Form["productId"]}");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);
                                
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Sell", new SellViewModel("error"));
                    }

                    break;
                }
            }
            return RedirectToAction("Sell");
        }

        [HttpGet] [ActionName("Transactions")]
        public IActionResult GetTransactions()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            var output = new List<Transaction>();
            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("GET"), $"{Request.Scheme}://{Request.Host}/Transactions/GetTransactionsByUserID/{userId}");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(_configuration["apiKey"]), "apiKey"}
            };
            request.Content = multipartContent;

            var response = httpClient.Send(request);
        
            response.EnsureSuccessStatusCode();

            using var streamReader = new StreamReader(response.Content.ReadAsStream());
            var objects = JsonSerializer.Deserialize<TransactionDTO[]>(streamReader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            foreach (var obj in objects)
            {
                ProductDTO product = null;
                UserDTO owner = null;
                UserDTO buyer = null;
                if (obj.Owner != null)
                {
                    owner = obj.Owner;
                    using var request2 = new HttpRequestMessage(new HttpMethod("GET"), $"{Request.Scheme}://{Request.Host}/Users/GetUser/{obj.BuyerID}");
                    var multipartContent2 = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request2.Content = multipartContent2;

                    var response2 = httpClient.Send(request2);

                    response2.EnsureSuccessStatusCode();
                    using var streamReader2 = new StreamReader(response2.Content.ReadAsStream());
                    buyer = JsonSerializer.Deserialize<UserDTO>(streamReader2.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                }
                else if (obj.Buyer != null)
                {
                    buyer = obj.Buyer;
                    using var request2 = new HttpRequestMessage(new HttpMethod("GET"), $"{Request.Scheme}://{Request.Host}/Users/GetUser/{obj.OwnerID}");
                    var multipartContent2 = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request2.Content = multipartContent2;

                    var response2 = httpClient.Send(request2);

                    response2.EnsureSuccessStatusCode();
                    using var streamReader2 = new StreamReader(response2.Content.ReadAsStream());
                    owner = JsonSerializer.Deserialize<UserDTO>(streamReader2.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                }
                        
                using (var request3 = new HttpRequestMessage(new HttpMethod("GET"), $"{Request.Scheme}://{Request.Host}/Products/GetProduct/{obj.ProductID}"))
                {
                    var response3 = httpClient.Send(request3);

                    response3.EnsureSuccessStatusCode();
                    using (var streamReader3 = new StreamReader(response3.Content.ReadAsStream()))
                    { product = JsonSerializer.Deserialize<ProductDTO>(streamReader3.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true}); }
                }

                output.Add(new Transaction(obj.TransactionID, product, owner, buyer, obj.LastUpdate, obj.State));
            }

            return View(new TransactionsViewModel(output));
        }
        
        [HttpPost] [ActionName("Transactions")]
        public IActionResult PostTransactions([FromForm] int transactionId, [FromForm] string button)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Index");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };
            
            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("PUT"), $"{Request.Scheme}://{Request.Host}/Transactions/UpdateState/{transactionId}");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(button == "close" ? "1" : "2"), "state"},
                {new StringContent(_configuration["apiKey"]), "apiKey"}
            };
            request.Content = multipartContent;
                        
            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return RedirectToAction("Transactions");
            }

            return RedirectToAction("Transactions");
        }

        [HttpGet] [ActionName("Product")]
        public IActionResult GetProduct(int value)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };
            
            ProductDTO product;
            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(new HttpMethod("GET"),
                $"{Request.Scheme}://{Request.Host}/Products/GetProduct/{value}");
            var response = httpClient.Send(request);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return RedirectToAction("Products");
            }

            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                product = JsonSerializer.Deserialize<ProductDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            }

            UserDTO owner;
            using var request2 = new HttpRequestMessage(new HttpMethod("GET"),
                $"{Request.Scheme}://{Request.Host}/Users/GetUser/{product.OwnerID}");
            var multipartContent = new MultipartFormDataContent
            {
                {new StringContent(_configuration["apiKey"]), "apiKey"}
            };
            request2.Content = multipartContent;
            response = httpClient.Send(request2);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return RedirectToAction("Products");
            }

            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                owner = JsonSerializer.Deserialize<UserDTO>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            }

            return View(new ProductViewModel(owner, product));
        }

        [HttpPost]
        [ActionName("Product")]
        public IActionResult PostProduct(int value, [FromForm] string button)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using var httpClient = new HttpClient(handler);
            switch (button)
            {
                case "delete":
                {
                    using var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{Request.Scheme}://{Request.Host}/Products/DeleteProduct/{value}");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);
                                
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("Sell");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Product", new { value });
                    }
                }
                case "buy":
                {
                    using var request = new HttpRequestMessage(new HttpMethod("POST"), $"{Request.Scheme}://{Request.Host}/Transactions/PostTransaction/");
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(value.ToString()), "ProductID"},
                        {new StringContent(Request.Form["ownerId"]), "OwnerID"},
                        {new StringContent(HttpContext.Session.GetInt32("userId").Value.ToString()), "BuyerID"},
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return RedirectToAction("Transactions");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Product", new { value });
                    }
                }
                default:
                    return RedirectToAction("Profile");
            }
        }

        [HttpGet]
        [ActionName("User")]
        public IActionResult GetUser(int value)
        {
            UserViewModel userViewModel;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (requestMessage, certificate, chain, policyErrors) => true
            };

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"),
                    $"{Request.Scheme}://{Request.Host}/Users/GetUser/{value}"))
                {
                    var multipartContent = new MultipartFormDataContent
                    {
                        {new StringContent(_configuration["apiKey"]), "apiKey"}
                    };
                    request.Content = multipartContent;

                    var response = httpClient.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Error");
                    }

                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        userViewModel = JsonSerializer.Deserialize<UserViewModel>(reader.ReadToEnd(), new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
                    }
                }
            }

            return View(userViewModel);
        }
    }
}