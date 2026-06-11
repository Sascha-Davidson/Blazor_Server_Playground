using Playground.Lib.Extensions;

namespace Playground.Lib.Enums;

/// <summary>
/// Defines supported HTML autocomplete field types based on the WHATWG
/// autocomplete specification.
///
/// Use these values with <see cref="AutoCompleteAttribute"/> to indicate
/// the type of information a form field represents, allowing browsers and
/// password managers to provide appropriate autofill suggestions.
/// </summary>
/// <remarks>
/// Supported categories:
/// <list type="bullet">
///   <item>
///     <description>
///       <b>Name and Organization</b>:
///       Name, HonorificPrefix, GivenName, AdditionalName, FamilyName,
///       HonorificSuffix, Nickname, OrganizationTitle, Organization
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Credentials</b>:
///       Username, NewPassword, CurrentPassword
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Address Information</b>:
///       StreetAddress, AddressLine1, AddressLine2, AddressLine3,
///       AddressLevel1-4, Country, CountryName, PostalCode
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Payment Information</b>:
///       CcName, CcGivenName, CcAdditionalName, CcFamilyName,
///       CcNumber, CcExp, CcExpMonth, CcExpYear, CcCsc, CcType
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Transaction Information</b>:
///       TransactionCurrency, TransactionAmount, TransactionLanguage
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Personal Information</b>:
///       Language, Bday, BdayDay, BdayMonth, BdayYear,
///       Sex, Url, Photo
///     </description>
///   </item>
///   <item>
///     <description>
///       <b>Contact Information</b>:
///       Tel, TelCountryCode, TelNational, TelAreaCode,
///       TelLocal, TelLocalPrefix, TelLocalSuffix,
///       TelExtension, Email, Impp
///     </description>
///   </item>
/// </list>
/// </remarks>
public enum AutoCompleteType
{
    // Name and Company --------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Full name of a person as a single field.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used when first and last name are not separated</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("name")]
    Name,

    /// <summary>
    /// Honorific prefix used before a person's name.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Examples include titles such as Mr, Mrs, Dr, or Prof</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("honorific-prefix")]
    HonorificPrefix,

    /// <summary>
    /// Given (first) name of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the person's first name in personal identity forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("given-name")]
    GivenName,

    /// <summary>
    /// Additional given name(s) of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used for middle names or secondary given names</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("additional-name")]
    AdditionalName,

    /// <summary>
    /// Family name (last name) of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the surname used for identification within a family</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("family-name")]
    FamilyName,

    /// <summary>
    /// Honorific suffix used after a person's name.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Examples include titles such as Jr, Sr, or academic/professional suffixes like PhD</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("honorific-suffix")]
    HonorificSuffix,

    /// <summary>
    /// Nickname or informal name of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used for an alternative or preferred informal name</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("nickname")]
    Nickname,

    /// <summary>
    /// Job title or position within an organization.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the person's role or title in a company or organization</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("organization-title")]
    OrganizationTitle,

    /// <summary>
    /// Name of a company, organization, or institution.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the employer or affiliated organization of a person</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("organization")]
    Organization,

    // Credentials -------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Username used for authentication or account identification.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the unique login name for a user account</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("username")]
    Username,

    /// <summary>
    /// New password entry field for account creation or password change.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used when a user is setting or updating a password</description>
    ///   </item>
    ///   <item>
    ///     <description>Browsers typically avoid autofilling existing saved passwords here</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("new-password")]
    NewPassword,

    /// <summary>
    /// Current password used for authentication.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used when the user must confirm their existing password</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically appears in login or account verification flows</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue ("current-password")]
    CurrentPassword,

    // Shipping Address --------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Full street address in a single field.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used when the entire address is entered as one line instead of separate fields</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically includes street name, house number, and optional unit or apartment</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping street-address")]
    StreetAddress,

    /// <summary>
    /// First line of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Typically contains street name and house number</description>
    ///   </item>
    ///   <item>
    ///     <description>Used as part of shipping or billing address forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-line1")]
    AddressLine1,

    /// <summary>
    /// Second line of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used for apartment, suite, unit, or building details</description>
    ///   </item>
    ///   <item>
    ///     <description>Optional field in most address forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-line2")]
    AddressLine2,

    /// <summary>
    /// Third line of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used for extended address information when additional detail is required</description>
    ///   </item>
    ///   <item>
    ///     <description>Rarely used in most standard address formats</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-line3")]
    AddressLine3,

    /// <summary>
    /// Lowest-level administrative area of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents a very local area such as a neighborhood or ward (where supported)</description>
    ///   </item>
    ///   <item>
    ///     <description>Rarely used and only applicable in some countries or addressing systems</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-level4")]
    AddressLevel4,

    /// <summary>
    /// Sub-region or district level of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents a smaller administrative area within a city or region</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in some countries for detailed address structuring</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-level3")]
    AddressLevel3,

    /// <summary>
    /// City or municipality of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the primary locality such as a city or town</description>
    ///   </item>
    ///   <item>
    ///     <description>Used as part of structured shipping or billing address forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-level2")]
    AddressLevel2,

    /// <summary>
    /// Highest-level administrative region of an address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Typically represents state, province, or region</description>
    ///   </item>
    ///   <item>
    ///     <description>Used as part of structured address forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping address-level1")]
    AddressLevel1,

    /// <summary>
    /// Country of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the country in shipping or billing forms</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically used as a dropdown selection of ISO country values</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping country")]
    Country,

    /// <summary>
    /// Postal or ZIP code of a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Used for mail routing and geographic delivery sorting</description>
    ///   </item>
    ///   <item>
    ///     <description>Format varies depending on the country (e.g. numeric or alphanumeric)</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping postal-code")]
    PostalCode,

    /// <summary>
    /// Full name of the country in a postal address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the country name in human-readable form</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in shipping and billing address forms where full country names are displayed</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("shipping country-name")]
    CountryName,

    // Payment Information -----------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Name printed on a payment card.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the cardholder's name as shown on credit or debit cards</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in payment and checkout forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-name")]
    CcName,

    /// <summary>
    /// Given (first) name of the cardholder.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the first name of the person owning the payment card</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in billing and payment forms when cardholder name is split into fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-given-name")]
    CcGivenName,

    /// <summary>
    /// Additional given name(s) of the cardholder.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents middle or additional given names of the cardholder</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in payment forms when full name is split into multiple fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-additional-name")]
    CcAdditionalName,

    /// <summary>
    /// Family (last) name of the cardholder.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the surname of the person owning the payment card</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in payment forms when cardholder name is split into components</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-family-name")]
    CcFamilyName,

    /// <summary>
    /// Payment card number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the full credit or debit card number used for payments</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in checkout and billing forms for processing transactions</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-number")]
    CcNumber,

    /// <summary>
    /// Credit card expiration date.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the expiry date of a payment card (usually month and year)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in checkout forms to validate card validity</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-exp")]
    CcExp,

    /// <summary>
    /// Credit card expiration month.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the month portion of the card expiration date</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in payment forms where expiry date is split into month and year</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-exp-month")]
    CcExpMonth,

    /// <summary>
    /// Credit card expiration year.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the year portion of the card expiration date</description>
    ///   </item>
    ///   <item>
    ///     <description>Used together with expiration month in payment forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-exp-year")]
    CcExpYear,

    /// <summary>
    /// Card security code (CSC / CVV).
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the security code printed on the back or front of a payment card</description>
    ///   </item>
    ///   <item>
    ///     <description>Used to verify card ownership during online transactions</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-exp-csc")]
    CcCsc,

    /// <summary>
    /// Type of payment card.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the card brand or network (e.g. Visa, Mastercard)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in payment forms to identify supported card types</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("cc-type")]
    CcType,

    // Transaction information:
    // The use case for this isn't a typical autofill. The expectation would be that these fields are hidden and
    // the user agent might "use this information to suggest a credit card that it knows has sufficient balance and
    // that supports the relevant currency."------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Currency used for a financial transaction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the currency of a payment or monetary value</description>
    ///   </item>
    ///   <item>
    ///     <description>Commonly used in checkout, billing, and financial forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("transaction-currency")]
    TransactionCurrency,

    /// <summary>
    /// Monetary amount of a transaction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the numeric value of a payment or financial transaction</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in checkout, billing, and financial processing forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("transaction-amount")]
    TransactionAmount,

    /// <summary>
    /// Language used for a transaction or user interaction.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the preferred language for processing or displaying transaction-related content</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically uses language codes such as "en", "nl", or "de"</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("transaction-language")]
    TransactionLanguage,

    // Personal information ----------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Preferred language of the user.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the user's chosen language for UI and content localization</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically uses ISO language codes such as "en", "nl", or "de"</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("language")]
    Language,

    /// <summary>
    /// Full birthdate of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the complete date of birth (day, month, and year)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in forms requiring age verification or identity validation</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("bday")]
    Bday,

    /// <summary>
    /// Day component of a person's birthdate.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the day of the month in a birthdate</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when birthdate is split into separate fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("bday-day")]
    BdayDay,

    /// <summary>
    /// Month component of a person's birthdate.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the month of birth (1–12)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when birthdate is split into separate fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("bday-month")]
    BdayMonth,

    /// <summary>
    /// Year component of a person's birthdate.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the year of birth</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when birthdate is split into separate fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("bday-year")]
    BdayYear,

    /// <summary>
    /// Biological sex or gender identity of a person.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the user's sex or gender as entered in a form</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically used in identity, profile, or demographic forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("sex")]
    Sex,

    /// <summary>
    /// URL or website address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents a web link such as a personal or company website</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in profile, contact, or social media fields</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("url")]
    Url,

    /// <summary>
    /// URL or file reference to a user photo.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents a profile image or avatar of the user</description>
    ///   </item>
    ///   <item>
    ///     <description>Typically used in profile settings or identity forms</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("photo")]
    Photo,

    // Home contact info -------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Full telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the user's primary contact phone number</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in contact and account forms for communication</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel")]
    Tel,

    /// <summary>
    /// Country calling code of a telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the international dialing prefix (e.g. +31, +1)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when phone numbers are split into structured parts</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-country-code")]
    TelCountryCode,

    /// <summary>
    /// National telephone number without country code.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the local phone number within a country</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when phone numbers are separated into country and national parts</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-national")]
    TelNational,

    /// <summary>
    /// Area code of a telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the regional or city dialing code within a country</description>
    ///   </item>
    ///   <item>
    ///     <description>Used when phone numbers are split into structured components</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-area-code")]
    TelAreaCode,

    /// <summary>
    /// Local part of a telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the main subscriber number excluding area and country code</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in structured telephone number inputs</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-local")]
    TelLocal,

    /// <summary>
    /// Local prefix of a telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the initial part of the local number before the line number</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in countries where phone numbers are split into prefix and line segments</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-local-prefix")]
    TelLocalPrefix,

    /// <summary>
    /// Local suffix of a telephone number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the final part of a local telephone number</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in countries where phone numbers are split into multiple segments</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-local-suffix")]
    TelLocalSuffix,

    /// <summary>
    /// Telephone extension number.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents an internal extension within a phone system or organization</description>
    ///   </item>
    ///   <item>
    ///     <description>Used in business environments with switchboards or PBX systems</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home tel-extension")]
    TelExtension,

    /// <summary>
    /// Email address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents the user's primary email contact</description>
    ///   </item>
    ///   <item>
    ///     <description>Used for authentication, notifications, and account communication</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home email")]
    Email,

    /// <summary>
    /// Instant messaging protocol address.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Represents a messaging contact identifier (e.g. chat or IM handle)</description>
    ///   </item>
    ///   <item>
    ///     <description>Used for services like XMPP, IRC, or other messaging protocols</description>
    ///   </item>
    /// </list>
    /// </remarks>
    [AutoCompleteValue("home impp")]
    Impp,
}

