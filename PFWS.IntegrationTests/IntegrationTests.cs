// using System.Text;
// using Newtonsoft.Json;
// using PFWS.BusinessLogicLayer.DTOs.Account;
// using PFWS.BusinessLogicLayer.DTOs.Auth;

// namespace PFWS.IntegrationTests;

// [TestFixture]
// public class IntegrationTests
// {
//     private CustomWebApplicationFactory _factory;
//     private HttpClient _client;

//     [SetUp]
//     public void SetUp()
//     {
//         _factory = new CustomWebApplicationFactory();
//         _client = _factory.CreateClient();
//     }

//     private async Task<string> AuthenticateAsync()
//     {
//         var loginRequest = new { username = "yaryna", password = "Yaryna123" };
//         var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
//         var response = await _client.PostAsync("/api/auth/login", content);
//         response.EnsureSuccessStatusCode();
//         var responseString = await response.Content.ReadAsStringAsync();
//         var token = JsonConvert.DeserializeObject<LoginedUserDto>(responseString).Token;
//         return token;
//     }

//     [Test, Order(1)]
//     public async Task GetAccountById_ReturnsCorrectData()
//     {
//         int testAccountId = 1;
//         var token = await AuthenticateAsync();
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

//         var response = await _client.GetAsync($"/api/accounts/{testAccountId}");

//         Assert.That(response.IsSuccessStatusCode, Is.True);
//         var account = JsonConvert.DeserializeObject<GetAccountDto>(await response.Content.ReadAsStringAsync());
//         Assert.That(account.Id, Is.EqualTo(testAccountId));
//     }

//     [Test, Order(2)]
//     public async Task GetAccounts_ReturnsCorrectData()
//     {
//         var token = await AuthenticateAsync();
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

//         var response = await _client.GetAsync($"/api/accounts");

//         Assert.That(response.IsSuccessStatusCode, Is.True);
//         var accounts = JsonConvert.DeserializeObject<List<GetAccountDto>>(await response.Content.ReadAsStringAsync());
//         Assert.That(accounts.Count, Is.EqualTo(3));
//     }

//     [Test, Order(3)]
//     public async Task AddAccount_ReturnsSuccess()
//     {
//         var token = await AuthenticateAsync();
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//         var newAccount = new { Name = "New account", Balance = 100 };
//         var content = new StringContent(JsonConvert.SerializeObject(newAccount), Encoding.UTF8, "application/json");

//         var response = await _client.PostAsync($"/api/accounts", content);

//         Assert.That(response.IsSuccessStatusCode, Is.True);
//     }

//     [Test, Order(4)]
//     public async Task UpdateAccount_ReturnsSuccess()
//     {
//         var token = await AuthenticateAsync();
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//         var updatedAccountId = 1;
//         var updatedAccount = new { Name = "Updated account" };
//         var content = new StringContent(JsonConvert.SerializeObject(updatedAccount), Encoding.UTF8, "application/json");

//         var response = await _client.PutAsync($"/api/accounts/{updatedAccountId}", content);

//         Assert.That(response.IsSuccessStatusCode, Is.True);
//     }

//     [Test, Order(5)]
//     public async Task DeleteAccount_ReturnsSuccess()
//     {
//         var token = await AuthenticateAsync();
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//         var accountId = 6;

//         var response = await _client.DeleteAsync($"/api/accounts/{accountId}");

//         Assert.That(response.IsSuccessStatusCode, Is.True);
//     }

//     [TearDown]
//     public void TearDown()
//     {
//         _client?.Dispose();
//         _factory?.Dispose();
//     }
// }
