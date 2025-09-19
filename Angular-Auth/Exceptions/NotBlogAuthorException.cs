using System;

namespace Angular_Auth.Exceptions;

public class NotBlogAuthorException : Exception {
    public NotBlogAuthorException(string message) : base(message) { }
}
