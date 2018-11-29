using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BooksController : ODataController
    {
        private BookStoreContext _db;

        public BooksController(BookStoreContext context)
        {
            _db = context;
            if (context.Books.Count() == 0)
            {
                foreach (var b in DataSource.GetBooks())
                {
                    context.Books.Add(b);
                    context.Presses.Add(b.Press);
                }
                context.SaveChanges();
            }
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_db.Books);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Id == key));
        }

        /// <summary>
        /// Get the book by inputted author.
        /// This method only has one parameter: an optional parameter. 
        /// This is the ideal way of writing a method for the behavior we want.
        /// ISSUE: Passing null or '' to nameOptional will return the '204 No Content' status. 
        ///        Debugging shows that the nameOptional parameter inside the method is null, 
        ///        despite being optional and having a standard value.
        ///        It's as if the value gets overwritten with null. 
        /// QUERY EXAMPLES: 
        ///        404 Not Found: odata/Books/GetBookByAuthorWithoutRequiredParameter
        ///        204 No Content: odata/Books/GetBookByAuthorWithoutRequiredParameter(nameOptional=null)
        /// </summary>
        /// <param name="nameOptional"></param>
        /// <returns></returns>
        [EnableQuery]
        public IActionResult GetBookByAuthorWithoutRequiredParameter(string nameOptional = "Mark Michaelis")
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Author == nameOptional));
        }

        /// <summary>
        /// Get the book by inputted author.
        /// The following method has one regular string parameter and one optional string parameter.
        /// ISSUE: This one works when you give a value (can be any string value) to nameRequired and '' or null to nameOptional
        ///        or when you didn't even include nameOptional in the odata query, it will work as well.
        /// QUERY EXAMPLES: 
        ///        Returns the book: odata/Books/GetBookByAuthorWithRequiredParameter(nameRequired=null)
        ///        204 No Content: odata/Books/GetBookByAuthorWithRequiredParameter(nameRequired=null, nameOptional=null)
        /// </summary>
        /// <param name="nameRequired"></param>
        /// <param name="nameOptional"></param>
        /// <returns></returns>
        [EnableQuery]
        public IActionResult GetBookByAuthorWithRequiredParameter(string nameRequired, string nameOptional = "Mark Michaelis")
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Author == nameOptional));
        }

        /// <summary>
        /// Get the book by inputted author.
        /// This one has two optional parameters.
        /// ISSUE: This one too requires at least one value (can be any string value for nameOptional2) to work.
        /// QUERY EXAMPLES:
        ///        404 Not Found: odata/Books/GetBookByAuthorWithTwoOptionalParameters
        ///        204 No Content: odata/Books/GetBookByAuthorWithTwoOptionalParameters(nameOptional1=null)
        ///        204 No Content: odata/Books/GetBookByAuthorWithTwoOptionalParameters(nameOptional1=null, nameOptional2=null)
        ///        204 No Content: odata/Books/GetBookByAuthorWithTwoOptionalParameters(nameOptional1=null, nameOptional2='Mark Michaelis')
        ///        Returns the book: odata/Books/GetBookByAuthorWithTwoOptionalParameters(nameOptional1='Mark Michaelis')
        ///        Returns the book: odata/Books/GetBookByAuthorWithTwoOptionalParameters(nameOptional2='Mark Michaelis')
        /// </summary>
        /// <param name="nameOptional1"></param>
        /// <param name="nameOptional2"></param>
        /// <returns></returns>
        [EnableQuery]
        public IActionResult GetBookByAuthorWithTwoOptionalParameters(string nameOptional1 = "Mark Michaelis", string nameOptional2 = "Mark Michaelis")
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Author == nameOptional1));
        }
    }
}
