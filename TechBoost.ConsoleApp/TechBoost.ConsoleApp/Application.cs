using Shared.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Domain.Contracts.Factories;
using Shared.Domain.Contracts.Helper;
using TechBoost.Domain.Contracts.Application;
using TechBoost.Domain.Contracts.TechBoost;
using TechBoost.Domain.Models;

namespace TechBoost.ConsoleApp
{
    /// <summary>
    /// Class that contains the application
    /// </summary>
    public class Application : IApplication
    {
        // constants to refer injected services and objects
        private readonly IFileService _fileService;
        private readonly ITextLineUtilityService _textLineUtilityService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly IBlobServiceFactory _blobServiceFactory;
        private readonly UserDetailsConfiguration _userDetailsConfigurations;
        private readonly ConsoleText _consoleText;

        public Application(
            IFileService fileService,
            ITextLineUtilityService textLineUtilityService,
            IUserDetailsService userDetailsService,
            IBlobServiceFactory blobServiceFactory,
            UserDetailsConfiguration userDetailsConfigurations,
            ConsoleText consoleText)
        {
            _fileService = fileService;
            _textLineUtilityService = textLineUtilityService;
            _userDetailsService = userDetailsService;
            _blobServiceFactory = blobServiceFactory;
            _userDetailsConfigurations = userDetailsConfigurations;
            _consoleText = consoleText;
        }
        public async Task Run() 
        {
            var unsortedUserDetails = await GetUserDetailsFromFile(_userDetailsConfigurations.UnsortedDetailsFilePath);

            if (unsortedUserDetails.Any())
            {
                var unsortedUserDetailsString = _textLineUtilityService.GetLinesAsSingleString(unsortedUserDetails);

                var users = _userDetailsService.GetOrderedUsersFromStringList(unsortedUserDetails);

                users.ForEach(u => Console.WriteLine($"{u.Name}, {u.Role}"));
;            }

            Console.WriteLine(_consoleText.ExitProgram);
            Console.ReadLine();
        }

        #region : private methods

        /// <summary>
        /// Method to read a list of names from a file at a configured path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>  Returns the list of names or and empty list  </returns>
        private async Task<List<string>> GetUserDetailsFromFile(string filePath)
        {
            List<string> userDetails = new List<string>();

            // try to read the name list from the specified path or get the list of names from a custom location
            try
            {
                userDetails = await _fileService.ReadAllLinesAsync(filePath);
            }
            catch (FileNotFoundException)
            {
                userDetails = await GetUserDetailsFromFileAtCustomLocation();
            }

            return userDetails;
        }

        /// <summary>
        /// Method to read a list of names from a file at a path given by the User via the console
        /// </summary>
        /// <returns> Returns the list of names or and empty list </returns>
        private async Task<List<string>> GetUserDetailsFromFileAtCustomLocation()
        {
            List<string> userDetails = new List<string>();

            // Get input from user to determine if to read file from a custom path or exit
            Console.WriteLine(_consoleText.InputFileNotInDefaultPath);
            var choice = Console.ReadKey();
            Console.WriteLine();

            if (choice.KeyChar == 'c')
            {
                string filePath = string.Empty;

                do
                {
                    Console.WriteLine(_consoleText.EnterFilePath);
                    filePath = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        userDetails = await GetUserDetailsFromFile(filePath);
                    }
                } while (string.IsNullOrWhiteSpace(filePath));
            }

            return userDetails;
        }

        /// <summary>
        /// Method to get user details from an Azure blob storage
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetUserDetailsFromBlobAsync()
        {
            List<string> userDetails = new List<string>();

            var blobService = _blobServiceFactory.CreateBlobService("techboosttest", "userdetails");

            using (var inStream = new StreamReader(await blobService.OpenReadBlobAsync("unsorted-names-list.txt")))
            {
                while (!inStream.EndOfStream)
                {
                    userDetails.Add(await inStream.ReadLineAsync());
                }
            }
            return userDetails;
        }

        #endregion
    }
}
