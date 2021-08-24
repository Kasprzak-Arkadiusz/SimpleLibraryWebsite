﻿using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SimpleLibraryWebsite.Services.Authorization
{
    public static class Operations
    {
        public static OperationAuthorizationRequirement Update = 
            new OperationAuthorizationRequirement {Name=Constants.UpdateOperationName}; 
        public static OperationAuthorizationRequirement Delete = 
            new OperationAuthorizationRequirement {Name=Constants.DeleteOperationName};
        public static OperationAuthorizationRequirement Return = 
            new OperationAuthorizationRequirement {Name=Constants.ReturnOperationName};
        public static OperationAuthorizationRequirement Borrow = 
            new OperationAuthorizationRequirement {Name=Constants.BorrowOperationName};
        public static OperationAuthorizationRequirement Fulfill = 
            new OperationAuthorizationRequirement {Name=Constants.FulfillOperationName};
    }

    public static class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ReturnOperationName = "Return";
        public static readonly string BorrowOperationName = "Borrow";
        public static readonly string FulfillOperationName = "Fulfill";
    }
}
