using Newtonsoft.Json;
using Product.Service.ViewModels.ProductViewModels;
using System.Net.Http.Headers;

namespace Product.TelegramBot
{
    public class ControlProduct
    {
        private string baseURL = "https://productweb20240415024052.azurewebsites.net/api/";

        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            var products = new List<ProductViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.GetAsync($"Products/GetAllProducts");
                getData.EnsureSuccessStatusCode();

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<ProductViewModel>>(results).OrderByDescending(f => f.Id).ToList();

                    return products;
                }
                else
                {
                    return null;
                }


            }
        }


        public async Task<List<(long Id, int SortNumber)>> GetPropertiesForAllProductsAsync()
        {
            var products = new List<ProductViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.GetAsync($"Products/GetAllProducts");
                getData.EnsureSuccessStatusCode();

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<ProductViewModel>>(results).OrderByDescending(f => f.Id).ToList();

                    return products
                        .Select(product => (product.Id, product.SortNumber))
                        .ToList();
                }
                else
                {
                    return null;
                }


            }
        }

        public async Task<Stream> DownloadVideoStreamAsync(string videoPartPath)
        {
            string apiUrl = $"{baseURL}Products/DownloadByVideoPartPath/{videoPartPath}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a stream and return it
                        return await response.Content.ReadAsStreamAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to download video. Status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<(string videoData, string videoFileName)> GetVideoDatasOfProduct(long id)
        {
            var product = new ProductViewModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getData = await client.GetAsync($"Products/GetProductById/{id}");
                getData.EnsureSuccessStatusCode();

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject<ProductViewModel>(results);
                    // Extract the file name from the video data file path
                    string videoFileName = Path.GetFileName(product!.VideoData);

                    // Return both the file path and the file name
                    return (product.VideoData, videoFileName);
                }
                else
                {
                    return (null, null);
                }
            }
        }
    }
}