// <auto-generated/>
#pragma warning disable CS0618
using ApiSdk.Book.Dewy.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace ApiSdk.Book.Dewy
{
    /// <summary>
    /// Builds and executes requests for operations under \Book\dewy
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.18.0")]
    public partial class DewyRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the ApiSdk.Book.dewy.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder"/></returns>
        public global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("dewyValue", position);
                return new global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the ApiSdk.Book.dewy.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("dewyValue", position);
                return new global::ApiSdk.Book.Dewy.Item.WithDewyValueItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::ApiSdk.Book.Dewy.DewyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DewyRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Book/dewy", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::ApiSdk.Book.Dewy.DewyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public DewyRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/Book/dewy", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
