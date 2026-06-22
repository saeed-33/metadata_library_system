#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Generate an Arabic PDF report from the changes summary.
"""

import os

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import cm
from reportlab.platypus import (
    SimpleDocTemplate,
    Paragraph,
    Spacer,
    PageBreak,
)
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib.enums import TA_RIGHT, TA_CENTER, TA_LEFT

import arabic_reshaper
from bidi.algorithm import get_display


# --------------------------------------------------
# Arabic helper
# --------------------------------------------------
def ar(text):
    """Reshape and apply bidi to Arabic text."""
    reshaped = arabic_reshaper.reshape(str(text))
    return get_display(reshaped)


# --------------------------------------------------
# Register Arabic Font
# --------------------------------------------------
FONT_CANDIDATES = [
    r"C:\Windows\Fonts\arial.ttf",
    r"C:\Windows\Fonts\tahoma.ttf",
    r"C:\Windows\Fonts\arialuni.ttf",
]

font_path = None

for f in FONT_CANDIDATES:
    if os.path.exists(f):
        font_path = f
        break

if not font_path:
    raise FileNotFoundError(
        "No Arabic font found. Install Arial/Tahoma or update FONT_CANDIDATES."
    )

pdfmetrics.registerFont(TTFont("Arabic", font_path))
pdfmetrics.registerFont(TTFont("ArabicBold", font_path))

# --------------------------------------------------
# Build document
# --------------------------------------------------
doc = SimpleDocTemplate(
    "CHANGES_REPORT.pdf",
    pagesize=A4,
    rightMargin=2 * cm,
    leftMargin=2 * cm,
    topMargin=2 * cm,
    bottomMargin=2 * cm,
)

styles = getSampleStyleSheet()

styles.add(
    ParagraphStyle(
        name="ArabicTitle",
        fontName="ArabicBold",
        fontSize=20,
        alignment=TA_CENTER,
        spaceAfter=20,
        leading=28,
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicSubtitle",
        fontName="Arabic",
        fontSize=12,
        alignment=TA_CENTER,
        spaceAfter=30,
        leading=20,
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicHeading1",
        fontName="ArabicBold",
        fontSize=16,
        alignment=TA_RIGHT,
        spaceAfter=12,
        spaceBefore=16,
        leading=24,
        textColor=colors.HexColor("#1a5276"),
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicHeading2",
        fontName="ArabicBold",
        fontSize=13,
        alignment=TA_RIGHT,
        spaceAfter=8,
        spaceBefore=12,
        leading=20,
        textColor=colors.HexColor("#2874a6"),
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicBody",
        fontName="Arabic",
        fontSize=11,
        alignment=TA_RIGHT,
        spaceAfter=8,
        leading=18,
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicCode",
        fontName="Courier",
        fontSize=9,
        alignment=TA_LEFT,
        spaceAfter=8,
        leading=14,
        leftIndent=10,
        rightIndent=10,
        backColor=colors.HexColor("#f4f4f4"),
        borderPadding=6,
    )
)

styles.add(
    ParagraphStyle(
        name="ArabicBullet",
        fontName="Arabic",
        fontSize=11,
        alignment=TA_RIGHT,
        spaceAfter=4,
        leading=18,
        rightIndent=20,
    )
)


# --------------------------------------------------
# Helper functions
# --------------------------------------------------
def p_ar(text, style="ArabicBody"):
    return Paragraph(ar(text), styles[style])


def h1(text):
    return Paragraph(ar(text), styles["ArabicHeading1"])


def h2(text):
    return Paragraph(ar(text), styles["ArabicHeading2"])


def code(text):
    return Paragraph(
        f"<font face='Courier'>{text}</font>",
        styles["ArabicCode"],
    )


# --------------------------------------------------
# Content - all project modifications
# --------------------------------------------------
story = []

story.append(
    Paragraph(
        ar("تقرير التعديلات على المشروع"),
        styles["ArabicTitle"],
    )
)

story.append(
    Paragraph(
        ar("استجابة لملاحظات المراجع على نظام CQRS"),
        styles["ArabicSubtitle"],
    )
)

story.append(Spacer(1, 0.5 * cm))

story.append(
    p_ar(
        "تم تنفيذ جميع الملاحظات الواردة من المراجع على نظام إدارة المكتبة. "
        "تم التأكد من نجاح البناء (dotnet build LibrarySystem.sln) بدون أي أخطاء برمجية."
    )
)

story.append(Spacer(1, 0.5 * cm))

# --------------------------------------------------
# 1. FluentValidation
# --------------------------------------------------
story.append(h1("1. استخدام FluentValidation بدلاً من throw new Exception"))

story.append(h2("ملفات Validators الجديدة"))
story.append(p_ar("تم إنشاء Validators للأوامر الجديدة:"))

validators = [
    "LibrarySystem.Application/Validators/Patrons/CreatePatronCommandValidator.cs",
    "LibrarySystem.Application/Validators/Patrons/UpdatePatronCommandValidator.cs",
    "LibrarySystem.Application/Validators/ItemCopies/CreateItemCopyCommandValidator.cs",
    "LibrarySystem.Application/Validators/ItemCopies/UpdateItemCopyCommandValidator.cs",
    "LibrarySystem.Application/Validators/Circulation/ReturnCommandValidator.cs",
]
for item in validators:
    story.append(
        Paragraph(
            f"<font face='Courier'>• {item}</font>",
            styles["ArabicBullet"],
        )
    )

story.append(h2("تسجيل الـ Validators في Dependency Injection"))
story.append(p_ar("تم تعديل ApplicationServiceRegistration.cs ليضيف:"))
story.append(code("services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());"))
story.append(p_ar("كما تم إضافة ValidationBehavior إلى MediatR pipeline:"))
story.append(code("cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));"))

story.append(h2("ملف ValidationBehavior الجديد"))
story.append(
    Paragraph(
        "<font face='Courier'>• LibrarySystem.Application/Behaviors/ValidationBehavior.cs</font>",
        styles["ArabicBullet"],
    )
)
story.append(p_ar("يقوم بتنفيذValidators تلقائياً قبل الوصول إلى Handler ويرمي ValidationException عند وجود أخطاء."))

story.append(h2("استبدال throw new Exception في الـ Handlers"))
handlers_changes = [
    "CreatePatronCommand.cs: شيلت التحقق من تكرار الرقم الوطني (صار بالـ Validator).",
    "UpdatePatronCommand.cs: throw new Exception(المستعير غير موجود) → return false;",
    "DeletePatronCommand.cs: throw new Exception → throw new InvalidOperationException.",
    "CreateItemCopyCommand.cs: شيلت التحقق من Barcode ووجود الكتاب (صار بالـ Validator).",
    "UpdateItemCopyCommand.cs: throw new Exception(النسخة غير موجودة) → return false;",
    "CheckoutCommandHandler.cs: جميع throw new Exception → throw new InvalidOperationException.",
    "ReturnCommand.cs: جميع throw new Exception → throw new InvalidOperationException.",
]
for item in handlers_changes:
    story.append(p_ar("• " + item, "ArabicBullet"))

story.append(p_ar("النتيجة: لم يعد هناك أي throw new Exception في الـ Solution بأكمله."))

story.append(h2("تحسين Exception Handling Middleware"))
story.append(p_ar("تم تعديل LibrarySystem.API/Middleware/ExceptionHandlingMiddleware.cs ليعالج:"))
story.append(p_ar("• ValidationException → 400 Bad Request مع رسائل الأخطاء", "ArabicBullet"))
story.append(p_ar("• InvalidOperationException → 400 Bad Request", "ArabicBullet"))
story.append(p_ar("كما تم تسجيله في Program.cs:"))
story.append(code("app.UseMiddleware<ExceptionHandlingMiddleware>();"))

story.append(PageBreak())

# --------------------------------------------------
# 2. DTOs
# --------------------------------------------------
story.append(h1("2. نقل الـ DTOs إلى المجلد الصحيح"))

story.append(h2("ملفات DTOs الجديدة"))
story.append(p_ar("تم إنشاء الملفات التالية:"))

dtos = [
    "LibrarySystem.Application/DTOs/Values/CreateValueRequest.cs",
    "LibrarySystem.Application/DTOs/ResourceTemplates/TemplatePropertyRequest.cs",
    "LibrarySystem.Application/DTOs/Media/UploadMediaRequestDto.cs",
]
for item in dtos:
    story.append(
        Paragraph(
            f"<font face='Courier'>• {item}</font>",
            styles["ArabicBullet"],
        )
    )

story.append(h2("الملفات المحدثة"))
updated_dto_files = [
    "CreateItemCommand.cs: إزالة CreateValueRequest المدمج وإضافة using.",
    "CreateMediaCommand.cs: إضافة using LibrarySystem.Application.DTOs.Values.",
    "UpdateItemCommand.cs: إضافة using وإصلاح namespace المفقود (كان global namespace).",
    "UpdateMediaComman.cs: إضافة using.",
    "UpdateTemplatePropertiesCommand.cs: إزالة TemplatePropertyRequest المدمج.",
    "MediaController.cs: إزالة UploadMediaRequestDto المدمج.",
    "ItemMappingProfile.cs: إضافة using LibrarySystem.Application.DTOs.Values.",
    "PropertiesController.cs: تحديث using لـ GetSearchableFieldsQuery.",
]
for item in updated_dto_files:
    story.append(p_ar("• " + item, "ArabicBullet"))

story.append(h2("نقل الـ Query"))
story.append(p_ar("تم نقل GetSearchableFieldsQuery.cs من Commands/Properties/ إلى Queries/Properties/."))

# --------------------------------------------------
# 3. Mappings
# --------------------------------------------------
story.append(h1("3. تعديلات الـ Mappings"))
story.append(p_ar("تم إنشاء أوامر واستعلامات جديدة تستخدم Mapping Profiles الموجودة:"))
mappings = [
    "GetAllPatronsWithDeletedQuery تستخدم Patron -> PatronAdminResponse",
    "GetAllItemCopiesWithDeletedQuery تستخدم ItemCopy -> ItemCopyAdminResponse",
    "UndeletePatronCommand و UndeleteItemCopyCommand تستخدمان FindWithDeletedAsync",
]
for item in mappings:
    story.append(p_ar("• " + item, "ArabicBullet"))

story.append(PageBreak())

# --------------------------------------------------
# 4. Authorization
# --------------------------------------------------
story.append(h1("4. تعديلات الـ Authorization"))

story.append(h2("إضافة [Authorize] للـ Controllers الناقصة"))
story.append(p_ar("تم إضافة [Authorize] على مستوى الـ Class للـ Controllers:"))
auth_controllers = [
    "ItemsController",
    "ItemCopiesController",
    "MediaController",
    "ItemSetsController",
    "PropertiesController",
    "ResourceTemplatesController",
    "VocabulariesController",
    "FilesController",
]
for item in auth_controllers:
    story.append(
        Paragraph(
            f"<font face='Courier'>• {item}</font>",
            styles["ArabicBullet"],
        )
    )

story.append(h2("حماية Undelete بصلاحية Admin"))
story.append(p_ar("تم إضافة [Authorize(Roles = SystemRoles.Admin)] على جميع نقاط النهاية Undelete/Undelet:"))
undelete_controllers = [
    "ItemsController",
    "ItemSetsController",
    "MediaController",
    "PropertiesController",
    "ResourceTemplatesController",
    "VocabulariesController",
    "PatronsController (الجديد)",
    "ItemCopiesController (الجديد)",
]
for item in undelete_controllers:
    story.append(p_ar("• " + item, "ArabicBullet"))

# --------------------------------------------------
# 5. Undelete + AllWithDeleted
# --------------------------------------------------
story.append(h1("5. إضافة GET AllWithDeleted و PUT Undelete"))

story.append(h2("PatronsController"))
story.append(p_ar("• GET api/patrons/AllWithDeleted → GetAllPatronsWithDeletedQuery"))
story.append(p_ar("• PUT api/patrons/Undelete/{id} → UndeletePatronCommand"))

story.append(h2("ItemCopiesController"))
story.append(p_ar("• GET api/item-copies/AllWithDeleted → GetAllItemCopiesWithDeletedQuery"))
story.append(p_ar("• PUT api/item-copies/Undelete/{id} → UndeleteItemCopyCommand"))

story.append(h2("ملفات Commands/Queries الجديدة"))
new_files = [
    "LibrarySystem.Application/Queries/Patrons/GetAllPatronsWithDeletedQuery.cs",
    "LibrarySystem.Application/Commands/Patrons/UndeletePatronCommand.cs",
    "LibrarySystem.Application/Queries/ItemCopies/GetAllItemCopiesWithDeletedQuery.cs",
    "LibrarySystem.Application/Commands/ItemCopies/UndeleteItemCopyCommand.cs",
]
for item in new_files:
    story.append(
        Paragraph(
            f"<font face='Courier'>• {item}</font>",
            styles["ArabicBullet"],
        )
    )

story.append(PageBreak())

# --------------------------------------------------
# 6. Additional fixes
# --------------------------------------------------
story.append(h1("6. إصلاحات إضافية"))
extras = [
    "إصلاح namespace المفقود في UpdateItemCommand.cs.",
    "إصلاح warning الـ null reference في MediaController.cs عند Deserialize الـ ValuesJson.",
    "ترتيب using statements في الملفات المعدلة.",
]
for item in extras:
    story.append(p_ar("• " + item, "ArabicBullet"))

# --------------------------------------------------
# 7. Build result
# --------------------------------------------------
story.append(h1("7. نتيجة البناء"))
story.append(p_ar("تم تنفيذ الأمر: dotnet build LibrarySystem.sln"))
story.append(p_ar("النتيجة: نجاح البناء بدون أخطاء (0 Error(s)). تبقت بعض التحذيرات القديمة المتعلقة بثغرات معروفة في حزم AutoMapper و SixLabors.ImageSharp."))

story.append(Spacer(1, 1 * cm))

story.append(
    Paragraph(
        ar("--- نهاية التقرير ---"),
        styles["ArabicSubtitle"],
    )
)

# --------------------------------------------------
# Build PDF
# --------------------------------------------------
try:
    doc.build(story)
    print("PDF generated successfully: CHANGES_REPORT.pdf")

except Exception as e:
    print("PDF generation failed:")
    print(str(e))
    raise