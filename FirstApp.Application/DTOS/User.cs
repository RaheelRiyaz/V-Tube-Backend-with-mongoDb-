using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public record UserRequest
     (
     string UserName,
     string Email,
     string Password
     );

    public record LoginResponse
        (
        string Id,
        string AccessToken,
        string RefreshToken
        );

    public record LoginRequest
    {
        [Required]
        public string UserName { get; init; } = null!;
        [Required]

        public string Password { get; init; } = null!;
    }

    public record ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; init; } = null!;

        [Required]
        public string NewPassword { get; init; } = null!;

        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; } = null!;
    }


    public class UserResponse
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public IEnumerable<Address> Addresses { get; set; } = null!;
    }

    public record UserAddressRequest
    {
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
    }

    public record UpdateAddressRequest : UserAddressRequest
    {
        [Required]
        public string Id { get; set; } = null!;
    }

    public class UserWithAddresses
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public List<AddressResponse> Addresses { get; set; } = null!;
    }
    public class AddressResponse
    {

        public ObjectId Id { get; set; }
        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
    }
}
