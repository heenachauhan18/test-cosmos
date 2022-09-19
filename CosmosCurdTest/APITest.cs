using CosmosCRUD.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CosmosCurdTest
{
    [TestClass]
    public class APITest
    {

        private HttpClient httpClient;

        public APITest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateDefaultClient();
        }

        private async Task<HttpResponseMessage> CreateUser(string emailAddress)
        {

            UserRequestDTO userDTO = new UserRequestDTO
            {
                FirstName = "fName",
                LastName = "lName",
                MiddleName = "mName",
                PhoneNumber = "123456789",
                EmailAddress = $"{emailAddress}"
            };
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync<UserRequestDTO>("/api/users", userDTO);
            return httpResponseMessage;
        }

        [TestMethod]
        public async Task ShouldCreateUser()
        {
            string uuid = Guid.NewGuid().ToString();
            HttpResponseMessage httpResponseMessage = await CreateUser($"{uuid}@gmail.com");
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task ShouldThrowUserBadRequestException()
        {
            HttpResponseMessage httpResponseMessage = await CreateUser(null);
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task ShouldThrowUserAlreadyExistsException()
        {
            string uuid = Guid.NewGuid().ToString();
            string email = $"{uuid}@gmail.com";
            HttpResponseMessage httpResponseMessage = await CreateUser(email);
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.Created);

            HttpResponseMessage errorResponse = await CreateUser(email);
            Assert.AreEqual(errorResponse.StatusCode, HttpStatusCode.Conflict);
        }

        [TestMethod]
        public async Task ShouldGetUserByEmail()
        {
            string uuid = Guid.NewGuid().ToString();
            string email = $"{uuid}@gmail.com";
            HttpResponseMessage httpResponseMessage = await CreateUser(email);
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.Created);

            UserResponseDTO? response = await httpClient.GetFromJsonAsync<UserResponseDTO>($"/api/users?emailAddress={email}");
            Assert.AreEqual(response.emailAddress, email);
        }

        [TestMethod]
        public async Task ShouldThrowUserNotFoundException()
        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"/api/users?emailAddress=abc@gmail.com");
            Assert.AreEqual(httpResponseMessage.StatusCode, HttpStatusCode.NotFound);
        }

    }
}