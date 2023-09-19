using System;

#nullable enable
namespace CodingHelmet.Optional
{
    public struct Option<T> : IEquatable<Option<T>> where T : class
    {
        private T? _content;

        public static Option<T> Some(T obj) => new() { _content = obj };
        public static Option<T> None() => new();

        public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class =>
            new() { _content = _content is not null ? map(_content) : null };
        public readonly ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map) where TResult : struct =>
            _content is not null ? ValueOption<TResult>.Some(map(_content)) : ValueOption<TResult>.None();

        public readonly Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) where TResult : class =>
            _content is not null ? map(_content) : Option<TResult>.None();
        public readonly ValueOption<TResult> MapOptionalValue<TResult>(Func<T, ValueOption<TResult>> map) where TResult : struct =>
            _content is not null ? map(_content) : ValueOption<TResult>.None();

        public readonly T Reduce(T orElse) => _content ?? orElse;
        public readonly T Reduce(Func<T> orElse) => _content ?? orElse();

        public readonly Option<T> Where(Func<T, bool> predicate) =>
            _content is not null && predicate(_content) ? this : Option<T>.None();

        public readonly Option<T> WhereNot(Func<T, bool> predicate) =>
            _content is not null && !predicate(_content) ? this : Option<T>.None();

        public override readonly int GetHashCode() => _content?.GetHashCode() ?? 0;
        public override readonly bool Equals(object? other) => other is Option<T> option && Equals(option);

        public readonly bool Equals(Option<T> other) =>
            _content is null ? other._content is null
            : _content.Equals(other._content);

        public static bool operator ==(Option<T>? a, Option<T>? b) => a is null ? b is null : a.Equals(b);
        public static bool operator !=(Option<T>? a, Option<T>? b) => !(a == b);
    }
}