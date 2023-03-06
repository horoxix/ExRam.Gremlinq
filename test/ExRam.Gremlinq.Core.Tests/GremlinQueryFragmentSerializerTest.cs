﻿using System.Collections.Immutable;
using ExRam.Gremlinq.Core.Steps;
using ExRam.Gremlinq.Core.Serialization;
using ExRam.Gremlinq.Core.Transformation;

namespace ExRam.Gremlinq.Core.Tests
{
    public class GremlinQueryFragmentSerializerTest : GremlinqTestBase
    {
        public GremlinQueryFragmentSerializerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {

        }

        [Fact]
        public async Task Empty()
        {
            await Verify(Transformer.Identity
                .TransformTo<object>().From(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        }

        [Fact]
        public async Task Base_type()
        {
            await Verify(Transformer.Identity
                .Add<Step, object>((step, env, recurse) => new VStep(ImmutableArray.Create<object>("id")))
                .TransformTo<object>().From(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        }

        [Fact]
        public async Task Irrelevant()
        {
            await Verify(Transformer.Identity
                .Add<HasKeyStep, object>((step, env, recurse) => new HasLabelStep(ImmutableArray.Create("should not be here")))
                .TransformTo<object>().From(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        }

        //[Fact]
        //public async Task Override1()
        //{
        //    await Verify(GremlinQuerySerializer.Identity
        //        .Override<HasLabelStep>((step, env, recurse) => overridden(new HasLabelStep(step.Labels.Add("added label")), env, recurse))
        //        .Serialize(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        //}

        //[Fact]
        //public async Task Override2()
        //{
        //    await Verify(GremlinQuerySerializer.Identity
        //        .Override<HasLabelStep>((step, env, recurse) => overridden(new HasLabelStep(step.Labels.Add("added label override 1")), env, recurse))
        //        .Override<HasLabelStep>((step, env, recurse) => overridden(new HasLabelStep(step.Labels.Add("added label override 2")), env, recurse))
        //        .Serialize(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        //}

        [Fact]
        public async Task Recurse()
        {
            await Verify(Transformer.Identity
                .Add<HasLabelStep, object>((step, env, recurse) => recurse.TransformTo<object>().From(new VStep(ImmutableArray.Create<object>("id")), env))
                .TransformTo<object>().From(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        }

        //[Fact]
        //public async Task Recurse_to_previous_override()
        //{
        //    await Verify(GremlinQuerySerializer.Identity
        //        .Override<VStep>((step, env, recurse) => overridden(new VStep(step.Ids.Add("another id")), env, recurse))
        //        .Override<HasLabelStep>((step, env, recurse) => recurse.Serialize(new VStep(ImmutableArray.Create<object>("id")), env))
        //        .Serialize(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        //}

        //[Fact]
        //public async Task Recurse_to_later_override()
        //{
        //    await Verify(GremlinQuerySerializer.Identity
        //        .Override<HasLabelStep>((step, env, recurse) => recurse.Serialize(new VStep(ImmutableArray.Create<object>("id")), env))
        //        .Override<VStep>((step, env, overridden, recurse) => overridden(new VStep(step.Ids.Add("another id")), env, recurse))
        //        .Serialize(new HasLabelStep(ImmutableArray.Create("label")), GremlinQueryEnvironment.Empty));
        //}

        [Fact]
        public async Task AllSteps()
        {
            await Verify(
                TypeSystemTest.AllSteps
                    .Select(step => (
                        step.GetType(),
                        Serializer.Default
                            .TransformTo<object>().From(step, GremlinQueryEnvironment.Empty)))
                    .ToArray());
        }
    }
}
