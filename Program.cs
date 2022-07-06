using PokemonPRNG;

// 針結果
// 6時の位置を0として、時計回りに0~8
// https://twitter.com/e52301147/status/1544193587247972352
var result = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 0, 1 };

// 中断の手配
var cts = new CancellationTokenSource();
var ct = cts.Token;
Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
{
    cts.Cancel();
};

ulong done = 0;
var start = DateTime.Now;

// 全探索
Console.WriteLine("initialSeed\ttid  \tprogress");
Console.WriteLine("-----------\t-----\t--------");
Parallel.For(0x0, 0x100000000, new ParallelOptions() { CancellationToken = ct }, initialSeed =>
{
    var prng = new TinyMT((uint)initialSeed);
    // 13消費捨て
    for (var i = 0; i < 13; i++)
    {
        prng.Advance();
    }

    for (var i = 0; i < result.Length; i++)
    {
        var r = prng.GetRand() % 8;
        if (r != result[i])
        {
            Interlocked.Increment(ref done);
            return;
        }
    }
    Interlocked.Increment(ref done);

    var tid = prng.GetRand() & 0x0000FFFF;
    Console.WriteLine("   {0,8:X}\t{1,5:d}\t{2}%", initialSeed, tid, Math.Round(1.0 * done / 0x100000000 * 100, 2));
});
Console.WriteLine("100% done.\t{0}", DateTime.Now - start);
