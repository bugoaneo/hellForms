//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v12.3.10+d8df405
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	/// <summary>Одностраничник</summary>
	[PublishedModel("singlePage")]
	public partial class SinglePage : SysSeoPage, IBlockPageTemplate, ITopLevelNode
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		public new const string ModelTypeAlias = "singlePage";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<SinglePage, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public SinglePage(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Конструктор
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("contentBlocks")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockListModel ContentBlocks => global::Umbraco.Cms.Web.Common.PublishedModels.BlockPageTemplate.GetContentBlocks(this, _publishedValueFallback);

		///<summary>
		/// Переопределение ID клуба: Позволяет переопределить идентификатор клуба, откуда брать информацию, для всего раздела
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("clubId")]
		public virtual string ClubId => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetClubId(this, _publishedValueFallback);

		///<summary>
		/// Ссылки обратной связи: Всплывающий блок контактов (либо один контакт, либо раскрывающееся меню с контактами). Список возможных: _tel, _vk, _telegram, _whatsapp, _fb, _instagram, _youtube
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("contactLinks")]
		public virtual global::System.Collections.Generic.IEnumerable<global::Umbraco.Cms.Core.Models.Link> ContactLinks => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetContactLinks(this, _publishedValueFallback);

		///<summary>
		/// Фавикон для iOS: PNG в размере 180x180
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("faviconIOS")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops FaviconIos => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetFaviconIos(this, _publishedValueFallback);

		///<summary>
		/// Фавикон для современных браузеров: Нужен в формате svg - favicon.svg
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("faviconNB")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops FaviconNB => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetFaviconNB(this, _publishedValueFallback);

		///<summary>
		/// Подвал
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("footer")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockListModel Footer => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetFooter(this, _publishedValueFallback);

		///<summary>
		/// Шапка
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("header")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockListModel Header => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetHeader(this, _publishedValueFallback);

		///<summary>
		/// Счетчики и доп. мета в HEAD: Добавляются в конец мета тегов в {head} Используйте для размещения скриптов или дополнительных мета тегов. Применяются к главной странице и всем внутренним
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("metaExtended")]
		public virtual string MetaExtended => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetMetaExtended(this, _publishedValueFallback);

		///<summary>
		/// Мобильная шапка
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("mobileHeader")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockListModel MobileHeader => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetMobileHeader(this, _publishedValueFallback);

		///<summary>
		/// Изображение по умолчанию: Картинка-заглушка. Появится в блоках сайта, где не выбрано изображение.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("rootDefaultImage")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops RootDefaultImage => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetRootDefaultImage(this, _publishedValueFallback);

		///<summary>
		/// Показать ссылку "На главную": На главной странице или на одностраничнике переход наверх страницы. На других страницах - переход на главную
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[ImplementPropertyType("showNavigateTopButton")]
		public virtual bool ShowNavigateTopButton => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetShowNavigateTopButton(this, _publishedValueFallback);

		///<summary>
		/// Название сайта: Добавляется в заголовок вкладки браузера для всех страниц текущего сайта или раздела. Может использоваться в шаблонах писем
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("siteName")]
		public virtual string SiteName => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetSiteName(this, _publishedValueFallback);

		///<summary>
		/// Получатели писем: Список почтовых адресов через запятую. По этим адресам будут отправляться системные уведомления или сообщения с форм обратной связи
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("subscribers")]
		public virtual string Subscribers => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetSubscribers(this, _publishedValueFallback);

		///<summary>
		/// Тема сайта
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("websiteBaseTheme")]
		public virtual global::Newtonsoft.Json.Linq.JToken WebsiteBaseTheme => global::Umbraco.Cms.Web.Common.PublishedModels.TopLevelNode.GetWebsiteBaseTheme(this, _publishedValueFallback);
	}
}
