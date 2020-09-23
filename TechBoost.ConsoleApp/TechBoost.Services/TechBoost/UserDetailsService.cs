using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechBoost.Domain.Contracts.TechBoost;
using TechBoost.Domain.Models;
using static TechBoost.Domain.Constants.TechBoostConstants;

namespace TechBoost.Services.TechBoost
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly UserDetailsConfiguration _UserDetailsConfiguration;

        public UserDetailsService(UserDetailsConfiguration userDetailsConfiguration)
        {
            _UserDetailsConfiguration = userDetailsConfiguration;
        }
        public List<User> GetOrderedUsersFromStringList(List<string> unprocessedUsersDetails)
        {
            var processedUsersDetails = PreprocessUserDetails(unprocessedUsersDetails);
            List<User> users = new List<User>();

            if (processedUsersDetails.Any())
            {
                processedUsersDetails.ForEach(u => 
                {
                    var details = u.Split(_UserDetailsConfiguration.RoleSeparatorChar);

                    users.Add(new User {Name = details[0].Trim(), Role = details[1].ToUpper().Trim() });
                });
            }

            // Order users by their role and then by their name
            var orderedUsers = users.OrderBy(u => u.Role).ThenBy(u => u.Name).ToList();
            return orderedUsers;
        }

        #region: private methods

        /// <summary>
        /// Method to preprocess the name list in order to remove empty lines, whitspace lines and trim whitespaces
        /// </summary>
        /// <param name="unprocessedUsersDetails"> A string list of unprocessed user details </param>
        /// <returns> returns a string list of names, void of empty lines, leading and trailing whitespaces </returns>
        private List<string> PreprocessUserDetails(List<string> unprocessedUsersDetails)
        {
            List<string> processedUsersDetails = new List<string>();

            unprocessedUsersDetails.ForEach(detail =>
            {
                /* 
                 * Ignore white spaces, empty lines
                 * Ignore itmes with more than 1 comma or if the split items are empty
                 * Ignore if the second item when split is not a valid role
                 */
                if (!string.IsNullOrWhiteSpace(detail))
                {
                    var detailArray = detail.Split(_UserDetailsConfiguration.RoleSeparatorChar);

                    if (
                        detailArray.Length > 0 &&
                        detailArray.Length < 3 &&
                        !string.IsNullOrWhiteSpace(detailArray[0]) &&
                        !string.IsNullOrWhiteSpace(detailArray[1]) &&
                        IsValidRole(detailArray[1].ToUpper().Trim())
                    )
                    {
                        // Trim leading and trailing white spaces
                        processedUsersDetails.Add(detail.Trim());
                    }
                }
            });

            return processedUsersDetails;
        }

        private bool IsValidRole(string role)
        {
            return role == Role.ADMIN || role == Role.LECTURER || role == Role.STUDENT;
        }

        #endregion
    }
}
