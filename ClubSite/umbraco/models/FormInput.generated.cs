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
	/// <summary>Типовое поле</summary>
	[PublishedModel("formInput")]
	public partial class FormInput : PublishedElementModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		public new const string ModelTypeAlias = "formInput";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<FormInput, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public FormInput(IPublishedElement content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Дополнительные классы: Можно указать дополнительные css классы для поля.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("cssClass")]
		public virtual string CssClass => this.Value<string>(_publishedValueFallback, "cssClass");

		///<summary>
		/// Пояснение при ошибке: Текст пояснения, которое появляется при ошибке заполнения поля.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("errorDescription")]
		public virtual string ErrorDescription => this.Value<string>(_publishedValueFallback, "errorDescription");

		///<summary>
		/// Скрыть заголовок
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[ImplementPropertyType("hideLabel")]
		public virtual bool HideLabel => this.Value<bool>(_publishedValueFallback, "hideLabel");

		///<summary>
		/// Маска ввода: Маска ввода поля, например, +7 (999) 999-99-99. Для телефона и даты если маска пустая, используется маска по умолчанию из словаря (см. группу validate-*)
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("inputMask")]
		public virtual string InputMask => this.Value<string>(_publishedValueFallback, "inputMask");

		///<summary>
		/// Плейсхолдер
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("inputPlaceholder")]
		public virtual string InputPlaceholder => this.Value<string>(_publishedValueFallback, "inputPlaceholder");

		///<summary>
		/// Максимальный ввод: Максимальное число символов для ввода в поля
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[ImplementPropertyType("maxLenght")]
		public virtual int MaxLenght => this.Value<int>(_publishedValueFallback, "maxLenght");

		///<summary>
		/// Алиас: Должно состоять только из латинских букв и цифр. Не может начинаться с цифры. Должно быть уникальным в пределах формы. Используется для подстановки значений в уведомлениях
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("propertyAlias")]
		public virtual string PropertyAlias => this.Value<string>(_publishedValueFallback, "propertyAlias");

		///<summary>
		/// Валидация: Регулярное выражение для проверки значения. Пример, .+@.+\..+ - почта, ^(?=.{2,10}$)\d+ - число длиной от 2 до 10 цифр
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("propertyMask")]
		public virtual string PropertyMask => this.Value<string>(_publishedValueFallback, "propertyMask");

		///<summary>
		/// Заголовок поля: Отображается в форме. Должно быть понятным человеку
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("propertyName")]
		public virtual string PropertyName => this.Value<string>(_publishedValueFallback, "propertyName");

		///<summary>
		/// Обязательно к заполнению
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[ImplementPropertyType("propertyRequired")]
		public virtual bool PropertyRequired => this.Value<bool>(_publishedValueFallback, "propertyRequired");

		///<summary>
		/// Тип поля: Выберите вариант поля. Влияет на способ проверки поля и то, как в него вводится информация
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("propertyType")]
		public virtual string PropertyType => this.Value<string>(_publishedValueFallback, "propertyType");

		///<summary>
		/// Чекбокс установлен: Для флажков. Значение по умолчанию - выбрано
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "12.3.10+d8df405")]
		[ImplementPropertyType("selected")]
		public virtual bool Selected => this.Value<bool>(_publishedValueFallback, "selected");
	}
}
