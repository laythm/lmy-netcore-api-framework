using Common.Enums;
using System.Collections.Generic;

namespace Common.Models.Common
{
    public class BaseModel
    {
        public BaseModel()
        {
            Successes = new List<string>();
            Errors = new List<string>();
            Info = new List<string>();
            Warning = new List<string>();
        }

        #region Properties

        public bool HasError { get => Errors.Count > 0; }

        public bool HasSuccess { get => Successes.Count > 0; }

        #endregion

        #region Messages

        public List<string> Successes { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Info { get; set; }
        public List<string> Warning { get; set; }

        public void AddError(string message)
        {
            Errors.Add(message);
        }

        public void AddErrors(IEnumerable<string> messages)
        {
            Errors.AddRange(messages);
        }

        public void AddSuccess(string message)
        {
            Successes.Add(message);
        }

        public void AddInfo(string message)
        {
            Info.Add(message);
        }

        public void AddWarning(string message)
        {
            Warning.Add(message);
        }

        #endregion
    }
}
