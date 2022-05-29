using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*{
      "correlationId": "1653675488270:0.6383315177877893",
      "linkTypeId": "621fb8f9d508224e0888e796",
      "documentIds": [ "621fb8f9d508224e0888e7a0", "621fb8f9d508224e0888e7a5" ],
      "data": {}
    }*/
    public class NewLink
    {
        public string CorrelationId { get; set; } = "0";
        public string LinkTypeId { get; set; }
        public List<string> DocumentIds { get; set; }
        public object Data { get; set; } = new object();

        public NewLink(Link link)
        {
            LinkTypeId = link.LinkTypeId;
            DocumentIds = link.DocumentIds;
        }

        public NewLink(string currentDocumentId, string linkedDocumentId, string linkTypeId)
        {
            DocumentIds = new List<string>
            {
                currentDocumentId,
                linkedDocumentId
            };

            LinkTypeId = linkTypeId;
        }
    }
}
