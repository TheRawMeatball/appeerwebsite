using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using csharpwebsite.Server.Helpers;
using ImageMagick;

namespace csharpwebsite.Server.Services 
{  

    public interface IImageService
    {
        Task<string> GetSingleImage(IFormFileCollection uploadedFiles, string expectedIdentifier, string filePath = null);

        Task SquareImage(string filePath);
        string AvatarPath { get; }
        string QuestionPath { get; }

        Task Delete(string source);
    }

    public class ImageService : IImageService
    {

        private readonly AppSettings _configuration;

        public ImageService(IOptions<AppSettings> configuration)
        {
            _configuration = configuration.Value;
        }

        public string AvatarPath => _configuration.UploadPath + "avatars/";
        public string QuestionPath => _configuration.UploadPath + "questions/";
        public string ReplyImgPath => _configuration.UploadPath + "replyImgs/";

        public async Task Delete(string source)
        {
            await Task.Run(() => File.Delete(source));
        }

        public async Task<string> GetSingleImage(IFormFileCollection uploadedFiles, string expectedIdentifier, string _filePath = null) 
        {
            if (uploadedFiles.Count == 1)
            {
                //TODO: VERIFIY IMAGE TYPE        

                var f = uploadedFiles[0];
                if (f.Name != expectedIdentifier)
                {
                    throw new AppException("file is labelled incorrectly");
                }
                var filePath = _filePath ?? Guid.NewGuid() + Path.GetExtension(f.FileName);
                var fullFilePath = $"{_configuration.UploadPath}{f.Name}s/{filePath}";
                using (var stream = System.IO.File.Create(fullFilePath))
                {
                    await f.CopyToAsync(stream);
                }
                return filePath;
            }
            else if(uploadedFiles.Count > 0)
            {
                throw new AppException("Too Many Files");
            }

            return null;
        }

        public async Task SquareImage(string imgPath) 
        {
            await Task.Run(() => 
            {
                using(MagickImage img = new MagickImage(imgPath))
                {
                    var w = img.Width;
                    var h = img.Height;

                    var s = Math.Min(w,h);

                    img.Crop(s, s, Gravity.Center);

                    img.Write(imgPath);
                }
            });
        }
    }
}