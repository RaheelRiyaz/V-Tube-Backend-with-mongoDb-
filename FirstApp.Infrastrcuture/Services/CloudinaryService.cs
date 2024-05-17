using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Infrastrcuture.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Infrastrcuture.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryInstance cloudinary;
        private readonly Cloudinary _cloudinary;
        private readonly IStorageService storageService;

        public CloudinaryService
            (
            IOptions<CloudinaryInstance> cloudinary,
            IStorageService storageService
            )
        {
            this.cloudinary = cloudinary.Value;

            var account = new Account(
                  this.cloudinary.CloudName,
                  this.cloudinary.ApiKey,
                  this.cloudinary.ApiSecret
              );

            _cloudinary = new Cloudinary(account);
            this.storageService = storageService;
        }
        public async Task<RawUploadResult?> UploadFileOnCloudinary(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Handle upload result
                    if (uploadResult.Error is not null)
                        return null;

                    // File uploaded successfully
                    return uploadResult;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RawUploadResult>> UploadFilesOnCloudinary(IFormFileCollection files)
        {
            var filePaths = new List<RawUploadResult>();

            foreach (var file in files)
            {
                var res = await UploadFileOnCloudinary(file);

                if (res is not null)
                    filePaths.Add(res);
            }

            return filePaths;
        }


        public async Task<(RawUploadResult?, string?)> UploadVideoFileOnCloudinary(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Handle upload result
                    if (uploadResult.Error is not null)
                        return (null, null);

                    var filePath = await storageService.SaveLocalFile(file);
                    var duration = storageService.GetVideoDuration(filePath);
                    var deletedRes = storageService.DeleteLocalFile(filePath);

                    // File uploaded successfully
                    return (uploadResult, duration);
                }
            }
            catch
            {
                return (null, null);
            }
        }

    }
}
