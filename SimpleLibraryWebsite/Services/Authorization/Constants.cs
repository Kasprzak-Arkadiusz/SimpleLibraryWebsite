using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SimpleLibraryWebsite.Services.Authorization
{
    public static class Operations
    {
        public static readonly OperationAuthorizationRequirement Update = 
            new() {Name=Constants.UpdateOperationName}; 
        public static readonly OperationAuthorizationRequirement Delete = 
            new() {Name=Constants.DeleteOperationName};
        public static readonly OperationAuthorizationRequirement Return = 
            new() {Name=Constants.ReturnOperationName};
        public static readonly OperationAuthorizationRequirement Borrow = 
            new() {Name=Constants.BorrowOperationName};
        public static readonly OperationAuthorizationRequirement Fulfill = 
            new() {Name=Constants.FulfillOperationName};
    }

    public static class Constants
    {
        public const string CreateOperationName = "Create";
        public const string ReadOperationName = "Read";
        public const string UpdateOperationName = "Update";
        public const string DeleteOperationName = "Delete";
        public const string ReturnOperationName = "Return";
        public const string BorrowOperationName = "Borrow";
        public const string FulfillOperationName = "Fulfill";
    }
}
