using BookAPI.Models;
using BookAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Text;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public ExcelController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [Route("ReadFile")]
        [HttpPost]
        public async Task<string> ReadFile(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                #region Variable Declaration
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                Stream FileStream = null;
                #endregion

                #region Save Book Detail From Excel

                if (file.Length > 0)
                {

                    FileStream = file.OpenReadStream();

                    if (file != null && FileStream != null)
                    {
                        if (file.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (file.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else
                            message = "The file format is not supported.";

                        dsexcelRecords = reader.AsDataSet();
                        reader.Close();

                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            DataTable dtBookRecords = dsexcelRecords.Tables[0];
                            for (int i = 0; i < dtBookRecords.Rows.Count; i++)
                            {
                                Book objBook = new Book();
                                objBook.Id = Convert.ToInt32(dtBookRecords.Rows[i][0]);
                                objBook.Title = Convert.ToString(dtBookRecords.Rows[i][1]);
                                objBook.Author = Convert.ToString(dtBookRecords.Rows[i][2]);
                                objBook.Description = Convert.ToString(dtBookRecords.Rows[i][3]);
                                var newBook = await _bookRepository.Create(objBook);

                                if (newBook != null)
                                    message = "The Excel file has been successfully uploaded.";
                                else
                                    message = "Something Went Wrong!, The Excel file uploaded has failed.";
                            }
                        }
                        else
                            message = "Selected file is empty.";
                    }
                    else
                        message = "Invalid File.";
                }
                else
                    message = "Bad request.";

                return message;
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
