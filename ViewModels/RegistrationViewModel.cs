using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace robstagram.ViewModels
{
    /// <summary>
    /// RegistrationViewModel class for registering new users
    /// </summary>
    public class RegistrationViewModel
    {
        /// <summary>
        /// User email and login name
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// User firstname
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// User lastname
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// User location
        /// </summary>
        public string Location { get; set; }
    }
}
