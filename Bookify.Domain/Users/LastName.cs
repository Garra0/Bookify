namespace Bookify.Domain.Users;

// التعديل: تم تغيير الكلاس إلى readonly record struct.
// السبب: لتجنب تخصيص الذاكرة في الـ Heap وتقليل العبء على الـ Garbage Collector (GC)، 
// لكونه كائن قيمة بسيط يحتوي على حقل واحد فقط.
public readonly record struct LastName(string Value);
