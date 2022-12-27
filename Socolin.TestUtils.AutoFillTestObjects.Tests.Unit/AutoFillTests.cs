using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Socolin.TestUtils.AutoFillTestObjects.Tests.Unit;

public class AutoFillTests
{
    private class TestIntClass
    {
        public int IntValue { get; set; }
        public short ShortValue { get; set; }
        public char CharValue { get; set; }
        public long LongValue { get; set; }
        public byte ByteValue { get; set; }
        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }
        public bool BoolValue { get; set; }
    }

    [Test]
    public void AutoFillIntegerValues_IncrementByOne()
    {
        var element = AutoFill<TestIntClass>.One();

        Assert.That(element.IntValue, Is.EqualTo(1));
        Assert.That(element.ShortValue, Is.EqualTo(2));
        Assert.That(element.CharValue, Is.EqualTo(3));
        Assert.That(element.LongValue, Is.EqualTo(4));
        Assert.That(element.ByteValue, Is.EqualTo(5));
        Assert.That(element.FloatValue, Is.EqualTo(6));
        Assert.That(element.DoubleValue, Is.EqualTo(7));
        Assert.That(element.DecimalValue, Is.EqualTo(8));
        Assert.That(element.BoolValue, Is.EqualTo(true));
    }

    private class TestStringClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MultipleWordProperty { get; set; }
    }

    [Test]
    public void AutoFillStringValue_PrefixVariableNameWithSome()
    {
        var element = AutoFill<TestStringClass>.One();

        Assert.That(element.Name, Does.Match("^some-name$"));
        Assert.That(element.Description, Does.Match("^some-description$"));
        Assert.That(element.MultipleWordProperty, Does.Match("^some-multiple-word-property$"));
    }

    [Test]
    public void AutoFillStringValue_WithRandomFlag_SuffixRandomValue()
    {
        var element = AutoFill<TestStringClass>.One(AutoFillFlags.RandomizeString);

        Assert.That(element.Name, Does.Match("^some-name-([a-f0-9]{16})$"));
        Assert.That(element.Description, Does.Match("^some-description-([a-f0-9]{16})$"));
        Assert.That(element.MultipleWordProperty, Does.Match("^some-multiple-word-property-([a-f0-9]{16})$"));
    }

    private class TestNullable
    {
        public int? IntValue { get; set; }
        public char? ShortValue { get; set; }
        public byte? CharValue { get; set; }
        public short? LongValue { get; set; }
        public long? ByteValue { get; set; }
        public float? FloatValue { get; set; }
        public double? DoubleValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public bool? BoolValue { get; set; }
    }

    [Test]
    public void AutoFillNullableIntegerValues_IncrementByOne()
    {
        var element = AutoFill<TestNullable>.One();

        Assert.That(element.IntValue, Is.EqualTo(1));
        Assert.That(element.ShortValue, Is.EqualTo(2));
        Assert.That(element.CharValue, Is.EqualTo(3));
        Assert.That(element.LongValue, Is.EqualTo(4));
        Assert.That(element.ByteValue, Is.EqualTo(5));
        Assert.That(element.FloatValue, Is.EqualTo(6));
        Assert.That(element.DoubleValue, Is.EqualTo(7));
        Assert.That(element.DecimalValue, Is.EqualTo(8));
        Assert.That(element.BoolValue, Is.EqualTo(true));
    }

    private class TestList
    {
        public List<int> IntValues { get; set; }
        public List<string> StringValues { get; set; }
    }

    [Test]
    public void AutoFillList_FillWith3Elements()
    {
        var element = AutoFill<TestList>.One();

        Assert.That(element.IntValues, Is.EquivalentTo(new[] {1, 2, 3}));
        Assert.That(element.StringValues, Is.EquivalentTo(new[] {"some-string-values0", "some-string-values1", "some-string-values2"}));
    }


    private class TestObjectType
    {
        public int IntValue { get; set; }
        public TestObjectSubType SubObject { get; set; }
    }

    private class TestObjectSubType
    {
        public int SubIntValue { get; set; }
    }

    [Test]
    public void AutoFillObject_ContinueIncrementing()
    {
        var element = AutoFill<TestObjectType>.One();

        Assert.That(element.IntValue, Is.EqualTo(1));
        Assert.That(element.SubObject, Is.Not.Null);
        Assert.That(element.SubObject.SubIntValue, Is.EqualTo(2));
    }

    private class TestRecursive
    {
        public TestRecursive Recursive { get; set; }
    }

    [Test]
    public void AutoFillRecursiveObject_StopWhenMaxDepthIsReached()
    {
        var element = AutoFill<TestRecursive>.One(settings: new AutoFillSettings {MaxDepth = 1});

        Assert.That(element.Recursive.Recursive, Is.Null);
    }

    private class TestIgnoring
    {
        public int? Value { get; set; }
        public int? Ignored { get; set; }
        public TestIgnoringRecurse Child { get; set; }
    }

    private class TestIgnoringRecurse
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public int? Value { get; set; }
        public int? SubIgnored { get; set; }
    }

    [Test]
    public void AutoFill_DoNotFilIgnoredFields()
    {
        var element = AutoFill<TestIgnoring>.One(ignoring: (i) => new {i.Ignored, i.Child.SubIgnored});

        Assert.That(element.Value, Is.EqualTo(1));
        Assert.That(element.Ignored, Is.Null);
        Assert.That(element.Child.SubIgnored, Is.Null);
    }
}