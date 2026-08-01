namespace Bookify.Domain.Apartments;


// التعديل: تم تغيير الكلاس إلى readonly record struct.
// السبب: لتجنب تخصيص الذاكرة في الـ Heap (Heap Allocation) وتقليل جهد الـ Garbage Collector (GC)، 
// حيث يتم الآن تمثيله كـ Value Type في الـ Stack لأنه يحتوي على خاصية واحدة فقط.
public readonly record struct Name(string Value);

