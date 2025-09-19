using System;

namespace Angular_Auth.Exceptions;

public class BlogNotFoundException : Exception {
    public BlogNotFoundException(string message) : base(message) { }
}
