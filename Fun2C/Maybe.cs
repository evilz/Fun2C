using System.Runtime.CompilerServices;

namespace Fun2C;

public abstract record Maybe<T>
{
    public static Maybe<T> Nothing => new Nothing<T>();
};
public sealed record Just<T>(T Data) : Maybe<T>;
public sealed record Nothing<T>() : Maybe<T>;


public static class MaybeExt
{
    public static Maybe<T> ToMaybe<T>(this T value) => new Just<T>(value);
    public static Maybe<T> ToJust<T>(this T value) => new Just<T>(value);
    
    public static Maybe<T> Nothing<T>() => new Nothing<T>();
    
    public static T GetValue<T>(this Maybe<T> maybe) => (maybe switch { Just<T> j => j.Data, _ => default });
    public static T GetValueOrThrow<T>(this Maybe<T> maybe) => (maybe switch { Just<T> j => j.Data, _ => throw new Exception("Maybe is None")})!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsJust<T>(this Maybe<T> maybe) => maybe switch { Just<T> => true, _ => false };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNone<T>(this Maybe<T> maybe) =>  maybe switch { Just<T> => false, _ => true };
    
    public static T DefaultValue<T>(this Maybe<T> maybe, T value) => maybe switch { Just<T> j => j.Data, _ => value };
    
    public static T DefaultWith<T>(this Maybe<T> maybe, Func<T> defThunk ) => maybe switch { Just<T> j => j.Data, _ => defThunk() };

    public static Maybe<T> OrElse<T>(this Maybe<T> maybe, Maybe<T> ifNone  ) => maybe switch { Just<T> => maybe, _ => ifNone };
    
    public static Maybe<T> OrElseWith<T>(this Maybe<T> maybe, Func<Maybe<T>> ifNoneThunk   ) => maybe switch { Just<T> => maybe, _ => ifNoneThunk() };

    public static int Count<T>(this Maybe<T> maybe, T value) => maybe switch { Just<T> => 1, _ => 0 };
   
    public static TState Fold<T,TState>(this Maybe<T> maybe, Func<TState,T,TState> folder, TState state ) => maybe switch { Just<T> j => folder(state,j.Data), _ => state };

    public static TState FoldBack<T,TState>(this Maybe<T> maybe, Func<T,TState,TState> folder, TState state ) => maybe switch { Just<T> j => folder(j.Data,state), _ => state };

    // Any in C#
    public static bool Exists<T,TState>(this Maybe<T> maybe, Func<T,bool> predicate ) => maybe switch { Just<T> j => predicate(j.Data), _ => false };
    
    public static bool ForAll<T,TState>(this Maybe<T> maybe, Func<T,bool> predicate ) => maybe switch { Just<T> j => predicate(j.Data), _ => true };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this Maybe<T> maybe, T value ) => maybe switch { Just<T> j => j.Data!.Equals(value), _ => false };

    public static void Iterate<T>(this Maybe<T> maybe, Action<T> action ) {
        if (maybe is Just<T> j)
            action(j.Data);
    }
    
    public static Maybe<U> Map<T, U>(this Maybe<T> maybe, Func<T, U> mapping ) => maybe switch { Just<T> j => new Just<U>(mapping (j.Data)), _ => new Nothing<U>() };

    public static Maybe<U> Map2<T, U>(this Maybe<T> maybe1, Maybe<T> maybe2 , Func<T,T, U> mapping ) =>
        (maybe1,maybe2) switch
        {
            (Just<T> j1,Just<T> j2) => mapping(j1.Data,j2.Data).ToJust(),
            _ => new Nothing<U>()
        };
    
    public static Maybe<U> Map3<T, U>(this Maybe<T> maybe1, Maybe<T> maybe2,Maybe<T> maybe3 , Func<T,T,T, U> mapping ) =>
        (maybe1,maybe2,maybe3) switch
        {
            (Just<T> j1,Just<T> j2, Just<T> j3) => mapping(j1.Data,j2.Data,j3.Data).ToJust(),
            _ => new Nothing<U>()
        };
    
    public static Maybe<U> Bind<T, U>(this Maybe<T> maybe, Func<T, Maybe<U>> binder) => maybe switch { Just<T> j => binder(j.Data), _ => new Nothing<U>() };
    
    public static Maybe<U> Flatten<T,U>(this Maybe<T> maybe) where T : Maybe<U> => maybe switch { Just<T> j => j.Data, _ => new Nothing<U>() };

    // F# Filter
    public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate) => maybe switch { Just<T> j when predicate(j.Data) => maybe, _ => new Nothing<T>() };
  
    public static T[] ToArray<T>(this Maybe<T> maybe) => maybe switch { Just<T> j => new T[]{j.Data}, _ => Array.Empty<T>() };

    public static List<T> ToList<T>(this Maybe<T> maybe) => maybe switch { Just<T> j => new List<T>{j.Data}, _ => new List<T>() };
    
    public static T? ToNullable<T>(this Maybe<T> maybe)  where T : struct => maybe switch { Just<T> j => j.Data, _ => new T?() };
    
    public static Maybe<T> OfNullable<T>(this T? nullable) where T : struct => nullable.HasValue ? nullable.Value.ToJust() : new Nothing<T>();
    
    public static Maybe<T> OfObj<T>(this T? val)  => val is null ? new Nothing<T>() : new Just<T>(val);
    
    public static T? ToObj<T>(this Maybe<T> maybe) where  T : class => maybe switch { Just<T> j => j.Data, _ => null };
    
    public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
    {
        return _(); IEnumerable<T> _()
        {
            if (maybe.IsJust())
                yield return maybe.GetValue();
        }
    }
 
    
    
    
    public static Maybe<(T, U)> Merge<T, U>(this Maybe<T> first, Maybe<U> second) =>
        (first,second) switch
        {
            (Just<T> j1,Just<U> j2) => new Just<(T,U)>((j1.Data,j2.Data)),
            _ => new Nothing<(T,U)>()
        };
    
    public static Maybe<T> Try<T>(Func<T> func)
    {
        try {
            return new Just<T>(func());
        }
        catch {
            return new Nothing<T>();
        }
    }
    
    public static void Match<T>(this Maybe<T> maybe, Action<T> onJust, Action onNothing)
    {
        if (maybe is Just<T> j)
            onJust(j.Data);
        else
            onNothing();
    }
    
    public static TRet Match<T,TRet>(this Maybe<T> maybe, Func<T, TRet> onJust, Func<TRet> onNothing) =>
        maybe switch
        {
            Just<T> j => onJust(j.Data),
            _ => onNothing()
        };
    


    // public static Maybe<TRight> FromEither<TLeft, TRight>(Either<TLeft, TRight> either)
    // {
    //     Guard.DisallowNull(nameof(either), either);
    //
    //     return (either.Tag == EitherType.Right) switch
    //     {
    //         true => Just(either.FromRight()),
    //         _ => Nothing<TRight>()
    //     };
    // }
    

    public static bool IsSome<T>(this Maybe<T> maybe) => maybe switch { Just<T> => true, _ => false };
}
