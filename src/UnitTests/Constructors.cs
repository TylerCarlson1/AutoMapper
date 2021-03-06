﻿using System;
using System.Linq.Expressions;
using Xunit;
using Should;

namespace AutoMapper.UnitTests
{
    namespace Constructors
    {
        public class When_mapping_an_optional_GUID_constructor : AutoMapperSpecBase
        {
            Destination _destination;

            public class Destination
            {
                public Destination(Guid id = default(Guid)) { Id = id; }
                public Guid Id { get; set; }
            }

            public class Source
            {
                public Guid Id { get; set; }
            }

            protected override MapperConfiguration Configuration
            {
                get
                {
                    return new MapperConfiguration(c=>c.CreateMap<Source, Destination>());
                }
            }

            protected override void Because_of()
            {
                _destination = Mapper.Map<Destination>(new Source());
            }

            [Fact]
            public void Should_map_ok()
            {
                _destination.Id.ShouldEqual(Guid.Empty);
            }
        }

        public class When_mapping_a_constructor_parameter_from_nested_members : AutoMapperSpecBase
        {
            private Destination _destination;

            public class Source
            {
                public NestedSource Nested { get; set; }
            }

            public class NestedSource
            {
                public int Foo { get; set; }
            }

            public class Destination
            {
                public int Foo { get; }

                public Destination(int foo)
                {
                    Foo = foo;
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>().ForCtorParam("foo", opt => opt.MapFrom(s => s.Nested.Foo));
            });

            protected override void Because_of()
            {
                _destination = Mapper.Map<Destination>(new Source { Nested = new NestedSource { Foo = 5 } });
            }

            [Fact]
            public void Should_map_the_constructor_argument()
            {
                _destination.Foo.ShouldEqual(5);
            }
        }

        public class When_the_destination_has_a_matching_constructor_with_optional_extra_parameters : AutoMapperSpecBase
        {
            private Destination _destination;

            public class Source
            {
                public int Foo { get; set; }
            }

            public class Destination
            {
                private readonly int _foo;

                public int Foo
                {
                    get { return _foo; }
                }

                public string Bar { get;}

                public Destination(int foo, string bar = "bar")
                {
                    _foo = foo;
                    Bar = bar;
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>();
            });

            protected override void Because_of()
            {
                _destination = Mapper.Map<Source, Destination>(new Source { Foo = 5 });
            }

            [Fact]
            public void Should_map_the_constructor_argument()
            {
                _destination.Foo.ShouldEqual(5);
                _destination.Bar.ShouldEqual("bar");
            }
        }

        public class When_mapping_to_an_object_with_a_constructor_with_a_matching_argument : AutoMapperSpecBase
        {
            private Dest _dest;

            public class Source
            {
                public int Foo { get; set; }
                public int Bar { get; set; }
            }

            public class Dest
            {
                private readonly int _foo;

                public int Foo
                {
                    get { return _foo; }
                }

                public int Bar { get; set; }

                public Dest(int foo)
                {
                    _foo = foo;
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Dest>();
            });

            protected override void Because_of()
            {
                Expression<Func<object, object>> ctor = (input) => new Dest((int)input);

                object o = ctor.Compile()(5);

                _dest = Mapper.Map<Source, Dest>(new Source { Foo = 5, Bar = 10 });
            }

            [Fact]
            public void Should_map_the_constructor_argument()
            {
                _dest.Foo.ShouldEqual(5);
            }

            [Fact]
            public void Should_map_the_existing_properties()
            {
                _dest.Bar.ShouldEqual(10);
            }
        }

        public class When_mapping_to_an_object_with_a_private_constructor : AutoMapperSpecBase
        {
            private Dest _dest;

            public class Source
            {
                public int Foo { get; set; }
            }

            public class Dest
            {
                private readonly int _foo;

                public int Foo
                {
                    get { return _foo; }
                }

                private Dest(int foo)
                {
                    _foo = foo;
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Dest>();
            });

            protected override void Because_of()
            {
                _dest = Mapper.Map<Source, Dest>(new Source { Foo = 5 });
            }

            [Fact]
            public void Should_map_the_constructor_argument()
            {
                _dest.Foo.ShouldEqual(5);
            }
        }

        public class When_mapping_to_an_object_using_service_location : AutoMapperSpecBase
        {
            private Dest _dest;

            public class Source
            {
                public int Foo { get; set; }
            }

            public class Dest
            {
                private int _foo;
                private readonly int _addend;

                public int Foo
                {
                    get { return _foo + _addend; }
                    set { _foo = value; }
                }

                public Dest(int addend)
                {
                    _addend = addend;
                }

                public Dest()
                    : this(0)
                {
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(t => new Dest(5));
                cfg.CreateMap<Source, Dest>()
                    .ConstructUsingServiceLocator();
            });

            protected override void Because_of()
            {
                _dest = Mapper.Map<Source, Dest>(new Source { Foo = 5 });
            }

            [Fact]
            public void Should_map_with_the_custom_constructor()
            {
                _dest.Foo.ShouldEqual(10);
            }
        }

        public class When_mapping_to_an_object_using_contextual_service_location : AutoMapperSpecBase
        {
            private Dest _dest;

            public class Source
            {
                public int Foo { get; set; }
            }

            public class Dest
            {
                private int _foo;
                private readonly int _addend;

                public int Foo
                {
                    get { return _foo + _addend; }
                    set { _foo = value; }
                }

                public Dest(int addend)
                {
                    _addend = addend;
                }

                public Dest()
                    : this(0)
                {
                }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(t => new Dest(5));
                cfg.CreateMap<Source, Dest>()
                    .ConstructUsingServiceLocator();
            });

            protected override void Because_of()
            {
                _dest = Mapper.Map<Source, Dest>(new Source { Foo = 5 }, opt => opt.ConstructServicesUsing(t => new Dest(6)));
            }

            [Fact]
            public void Should_map_with_the_custom_constructor()
            {
                _dest.Foo.ShouldEqual(11);
            }
        }

        public class When_mapping_to_an_object_with_multiple_constructors_and_constructor_mapping_is_disabled : AutoMapperSpecBase
        {
            private Dest _dest;

            public class Source
            {
                public int Foo { get; set; }
                public int Bar { get; set; }
            }

            public class Dest
            {
                public int Foo { get; set; }

                public int Bar { get; set; }

                public Dest(int foo)
                {
                    throw new NotImplementedException();
                }

                public Dest() { }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.DisableConstructorMapping();
                cfg.CreateMap<Source, Dest>();
            });

            protected override void Because_of()
            {
                _dest = Mapper.Map<Source, Dest>(new Source { Foo = 5, Bar = 10 });
            }

            [Fact]
            public void Should_map_the_existing_properties()
            {
                _dest.Foo.ShouldEqual(5);
                _dest.Bar.ShouldEqual(10);
            }
        }
        public class UsingMappingEngineToResolveConstructorArguments
        {
            [Fact]
            public void Should_resolve_constructor_arguments_using_mapping_engine()
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SourceBar, DestinationBar>();

                    cfg.CreateMap<SourceFoo, DestinationFoo>();
                });

                var sourceBar = new SourceBar("fooBar");
                var sourceFoo = new SourceFoo(sourceBar);

                var destinationFoo = config.CreateMapper().Map<DestinationFoo>(sourceFoo);

                destinationFoo.Bar.FooBar.ShouldEqual(sourceBar.FooBar);
            }


            public class DestinationFoo
            {
                private readonly DestinationBar _bar;

                public DestinationBar Bar
                {
                    get { return _bar; }
                }

                public DestinationFoo(DestinationBar bar)
                {
                    _bar = bar;
                }
            }

            public class DestinationBar
            {
                private readonly string _fooBar;

                public string FooBar
                {
                    get { return _fooBar; }
                }

                public DestinationBar(string fooBar)
                {
                    _fooBar = fooBar;
                }
            }

            public class SourceFoo
            {
                public SourceBar Bar { get; private set; }

                public SourceFoo(SourceBar bar)
                {
                    Bar = bar;
                }
            }

            public class SourceBar
            {
                public string FooBar { get; private set; }

                public SourceBar(string fooBar)
                {
                    FooBar = fooBar;
                }
            }
        }

        public class When_mapping_to_an_object_with_a_constructor_with_multiple_optional_arguments
        {
            [Fact]
            public void Should_resolve_constructor_when_args_are_optional()
            {

                var config = new MapperConfiguration(cfg => cfg.CreateMap<SourceFoo, DestinationFoo>());

                var sourceBar = new SourceBar("fooBar");
                var sourceFoo = new SourceFoo(sourceBar);

                var destinationFoo = config.CreateMapper().Map<DestinationFoo>(sourceFoo);

                destinationFoo.Bar.ShouldBeNull();
                destinationFoo.Str.ShouldEqual("hello");
            }


            public class DestinationFoo
            {
                private readonly DestinationBar _bar;
                private string _str;

                public DestinationBar Bar
                {
                    get { return _bar; }
                }

                public string Str
                {
                    get { return _str; }
                }

                public DestinationFoo(DestinationBar bar=null,string str="hello")
                {
                    _bar = bar;
                    _str = str;
                }
            }

            public class DestinationBar
            {
                private readonly string _fooBar;

                public string FooBar
                {
                    get { return _fooBar; }
                }

                public DestinationBar(string fooBar)
                {
                    _fooBar = fooBar;
                }
            }

            public class SourceFoo
            {
                public SourceBar Bar { get; private set; }

                public SourceFoo(SourceBar bar)
                {
                    Bar = bar;
                }
            }

            public class SourceBar
            {
                public string FooBar { get; private set; }

                public SourceBar(string fooBar)
                {
                    FooBar = fooBar;
                }
            }
        }


        public class When_mapping_to_an_object_with_a_constructor_with_single_optional_arguments
        {
            [Fact]
            public void Should_resolve_constructor_when_arg_is_optional()
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SourceFoo, DestinationFoo>());

                var sourceBar = new SourceBar("fooBar");
                var sourceFoo = new SourceFoo(sourceBar);

                var destinationFoo = config.CreateMapper().Map<DestinationFoo>(sourceFoo);

                destinationFoo.Bar.ShouldBeNull();
            }


            public class DestinationFoo
            {
                private readonly DestinationBar _bar;

                public DestinationBar Bar
                {
                    get { return _bar; }
                }

                public DestinationFoo(DestinationBar bar = null)
                {
                    _bar = bar;
                }
            }

            public class DestinationBar
            {
                private readonly string _fooBar;

                public string FooBar
                {
                    get { return _fooBar; }
                }

                public DestinationBar(string fooBar)
                {
                    _fooBar = fooBar;
                }
            }

            public class SourceFoo
            {
                public SourceBar Bar { get; private set; }

                public SourceFoo(SourceBar bar)
                {
                    Bar = bar;
                }
            }

            public class SourceBar
            {
                public string FooBar { get; private set; }

                public SourceBar(string fooBar)
                {
                    FooBar = fooBar;
                }
            }
        }

        public class When_mapping_to_an_object_with_a_constructor_with_string_optional_arguments
        {
            [Fact]
            public void Should_resolve_constructor_when_string_args_are_optional()
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SourceFoo, DestinationFoo>());

                var sourceBar = new SourceBar("fooBar");
                var sourceFoo = new SourceFoo(sourceBar);

                var destinationFoo = config.CreateMapper().Map<DestinationFoo>(sourceFoo);

                destinationFoo.A.ShouldEqual("a");
                destinationFoo.B.ShouldEqual("b");
                destinationFoo.C.ShouldEqual(3);
            }


            public class DestinationFoo
            {
                private string _a;
                private string _b;
                private int _c;
                public string A
                {
                    get { return _a; }
                }

                public string B
                {
                    get { return _b; }
                }

                public int C
                {
                    get { return _c; }
                }

                public DestinationFoo(string a = "a",string b="b", int c = 3)
                {
                    _a = a;
                    _b = b;
                    _c = c;
                }
            }

            public class DestinationBar
            {
                private readonly string _fooBar;

                public string FooBar
                {
                    get { return _fooBar; }
                }

                public DestinationBar(string fooBar)
                {
                    _fooBar = fooBar;
                }
            }

            public class SourceFoo
            {
                public SourceBar Bar { get; private set; }

                public SourceFoo(SourceBar bar)
                {
                    Bar = bar;
                }
            }

            public class SourceBar
            {
                public string FooBar { get; private set; }

                public SourceBar(string fooBar)
                {
                    FooBar = fooBar;
                }
            }
        }

        public class When_configuring_ctor_param_members : AutoMapperSpecBase
        {
            public class Source
            {
                public int Value { get; set; }
            }

            public class Dest
            {
                public Dest(int thing)
                {
                    Value1 = thing;
                }

                public int Value1 { get; }
            }

            protected override MapperConfiguration Configuration { get; } = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Dest>().ForCtorParam("thing", opt => opt.MapFrom(src => src.Value));
            });

            [Fact]
            public void Should_redirect_value()
            {
                var dest = Mapper.Map<Source, Dest>(new Source {Value = 5});

                dest.Value1.ShouldEqual(5);
            }
        }
    }
}