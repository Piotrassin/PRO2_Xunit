using Library.Entities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public BookBorrowInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBookBorrow = 1,
                    IdBook = 1,
                    IdUser = 1,
                    Comments = "Komentarz"
            });

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task AddBookBorrow_200Ok()
        {
            var newBookBorrowed = new BookBorrow
            {
                IdBookBorrow = 2,
                IdBook = 2,
                IdUser = 2,
                Comments = "Tajemniczy jegomość miał maskę i pachniał izoporopanolem"
            };

            var bookBorrowJson = new StringContent(JsonConvert.SerializeObject(newBookBorrowed), Encoding.UTF8, "application/json");
            var httpResp = _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", bookBorrowJson).Result;

            httpResp.EnsureSuccessStatusCode();

            using (var scope = _server.Host.Services.CreateScope())
            {
                var _baza = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                Assert.True(_baza.BookBorrow.Count() == 2);
            }

        }

        [Fact]
        public async Task UpdateBookBorrow_200Ok()
        {
            using (var scope = _server.Host.Services.CreateScope())
            {
                int IdToUpdate = 1; 

                var _baza = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                var bookBorrowToUpdate = _baza.BookBorrow.Where(x => x.IdBookBorrow == IdToUpdate).FirstOrDefault();
                bookBorrowToUpdate.Comments = "Liście Eukaliptusa dostarczają niewiele więcej kalorii ponad to ile potrzeba na ich strawienie";


                var bookBorrowToUpdateJSON = new StringContent(JsonConvert.SerializeObject(bookBorrowToUpdate), Encoding.UTF8, "application/json");
                var HttpResponse = _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/2", bookBorrowToUpdateJSON).Result;
                HttpResponse.EnsureSuccessStatusCode();


                Assert.True(
                    _baza.BookBorrow
                    .Where(x => x.IdBookBorrow == IdToUpdate)
                    .FirstOrDefault()
                    .Comments == "Liście Eukaliptusa dostarczają niewiele więcej kalorii ponad to ile potrzeba na ich strawienie"
                );
            }
        }

         
    }
}
