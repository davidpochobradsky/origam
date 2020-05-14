import {
  Container,
  ILifetime,
  TypeSymbol,
  pushCurrentContainer,
  popCurrentContainer,
  Func,
  getCreatorStack,
  getScopePath,
} from "./Container";

interface IDog {
  dogName: string;
}

const IDog = TypeSymbol<IDog>("IDog");

class Dog implements IDog {
  dogName = "Jamie";
}

interface ICat {
  catName: string;
  garden: IGarden;
}

const ICat = TypeSymbol<ICat>("ICat");

class Cat implements ICat {
  constructor(public garden = IGarden()) {}

  catName = "Garfield";
}

interface IFish {
  fishName: string;
  garden: IGarden;
}

const IFish = TypeSymbol<IFish>("IFish");

class Fish implements IFish {
  constructor(public garden = IGarden()) {}

  fishName = "Nemo";
}

interface IGarden {
  size: number;
}

const IGarden = TypeSymbol<IGarden>("IGarden");

class Garden implements IGarden {
  size = 150;
}

interface IHouse {
  houseCity: string;
  dog: IDog;
  cat: ICat;
  fish: IFish;
}

const IHouse = TypeSymbol<IHouse>("IHouse");

class House implements IHouse {
  constructor(public dog = IDog(), public cat = ICat(), public fish = IFish()) {}

  houseCity = "Kladno";
}

class A {
  constructor(public b = IB(), public c = IC()) {}
}

const IA = TypeSymbol<A>("IA");

class B {}

const IB = TypeSymbol<B>("IB");

class C {}

const IC = TypeSymbol<C>("IC");

class X {
  constructor(public a = Func(IA)()) {}
}

const IX = TypeSymbol<X>("IX");

class PluginA {
  k = 0;
}

class PluginB {
  k = 1;
}

class PluginC {
  k = 2;
}

interface IPlugin {
  k: number;
}

const IPlugin = TypeSymbol<{ k: number }>("IPlugin");
const IPluginA = TypeSymbol<any>("IPluginA");
const IPluginB = TypeSymbol<any>("IPluginB");
const IPluginC = TypeSymbol<any>("IPluginC");

const IPluginEnumeration = TypeSymbol<IPlugin[]>("IPluginEnumeration");

class PluginConsumer {
  constructor(public plugins = IPluginEnumeration()) {}
}

const IPluginConsumer = TypeSymbol<PluginConsumer>("IPluginConsumer");

function createContainerTransient() {
  const container = new Container({ defaultLifetime: ILifetime.PerDependency });
  container.registerClass(IDog, Dog);
  container.registerClass(ICat, Cat);
  container.registerClass(IHouse, House);
  container.registerClass(IGarden, Garden);
  container.registerClass(IFish, Fish);
  return container;
}

function createContainerSingle() {
  const container = new Container({ defaultLifetime: ILifetime.Single });
  container.registerClass(IDog, Dog);
  container.registerClass(ICat, Cat);
  container.registerClass(IHouse, House);
  container.register(IGarden, () => new Garden());
  container.register(IFish, () => new Fish());
  return container;
}

function createContainerScoped() {
  const container = new Container({ defaultLifetime: ILifetime.PerLifetimeScope });
  container.registerClass(IDog, Dog);
  return container;
}

function createContainerSingleWithScopedRegistration() {
  const container = new Container({ defaultLifetime: ILifetime.Single });
  container.registerClass(IDog, Dog).scopedInstance("myScope");
  container.registerClass(ICat, Cat);
  container.registerClass(IHouse, House);
  container.registerClass(IGarden, Garden);
  container.registerClass(IFish, Fish);
  return container;
}

describe("Container", () => {
  it("provides transient instances", () => {
    const container = createContainerTransient();

    const dogs = [container.resolve(IDog), container.resolve(IDog), container.resolve(IDog)];
    expect(dogs[0].dogName).toBe("Jamie");
    expect(dogs[1].dogName).toBe("Jamie");
    expect(dogs[2].dogName).toBe("Jamie");
    expect(dogs[0]).not.toBe(dogs[1]);
    expect(dogs[0]).not.toBe(dogs[2]);
  });

  it("provides single instances", () => {
    const container1 = createContainerSingle();
    const dogs = [container1.resolve(IDog), container1.resolve(IDog)];
    const container2 = container1.beginLifetimeScope();
    dogs.push(container2.resolve(IDog), container2.resolve(IDog));

    expect(dogs[0].dogName).toBe("Jamie");
    expect(dogs[1].dogName).toBe("Jamie");
    expect(dogs[2].dogName).toBe("Jamie");
    expect(dogs[3].dogName).toBe("Jamie");
    expect(dogs[0]).toBe(dogs[1]);
    expect(dogs[0]).toBe(dogs[2]);
    expect(dogs[0]).toBe(dogs[3]);
  });

  it("provides scoped instances", () => {
    const container1 = createContainerScoped();
    const dogs = [container1.resolve(IDog), container1.resolve(IDog)];
    const container2 = container1.beginLifetimeScope();
    dogs.push(container2.resolve(IDog), container2.resolve(IDog));

    expect(dogs[0].dogName).toBe("Jamie");
    expect(dogs[1].dogName).toBe("Jamie");
    expect(dogs[2].dogName).toBe("Jamie");
    expect(dogs[3].dogName).toBe("Jamie");
    expect(dogs[0]).toBe(dogs[1]);
    expect(dogs[2]).toBe(dogs[3]);
    expect(dogs[0]).not.toBe(dogs[2]);
  });

  it("provides instances scoped by scope name", () => {
    const container1 = createContainerSingleWithScopedRegistration();
    const container2 = container1.beginLifetimeScope();
    const container3 = container2.beginLifetimeScope("myScope");
    const container4 = container3.beginLifetimeScope();

    const d = container4.resolve(IDog);

    expect(() => container1.resolve(IDog)).toThrowError();
    expect(() => container2.resolve(IDog)).toThrowError();

    const dogs = [
      container3.resolve(IDog),
      container3.resolve(IDog),
      container4.resolve(IDog),
      container4.resolve(IDog),
    ];

    expect(dogs[0].dogName).toBe("Jamie");
    expect(dogs[1].dogName).toBe("Jamie");
    expect(dogs[2].dogName).toBe("Jamie");
    expect(dogs[3].dogName).toBe("Jamie");

    expect(dogs[0]).toBe(dogs[1]);
    expect(dogs[0]).toBe(dogs[2]);
    expect(dogs[0]).toBe(dogs[3]);
  });

  it("allows obtaining current scope path", () => {
    class K {
      constructor() {
        expect(getScopePath()).toEqual([c4, c3, c2, c1]);
      }
    }
    const IK = TypeSymbol<K>("IK");

    const c1 = new Container({ defaultLifetime: ILifetime.Single });
    c1.registerClass(IK, K);
    const c2 = c1.beginLifetimeScope("l1");
    const c3 = c2.beginLifetimeScope("l2");
    const c4 = c3.beginLifetimeScope("l3");
    c4.resolve(IK);
  });

  it("resolves instances by calling their symbol", () => {
    const container = createContainerTransient();
    pushCurrentContainer(container);

    const dogs = [IDog(), IDog(), IDog()];
    expect(dogs[0].dogName).toBe("Jamie");
    expect(dogs[1].dogName).toBe("Jamie");
    expect(dogs[2].dogName).toBe("Jamie");
    expect(dogs[0]).not.toBe(dogs[1]);
    expect(dogs[0]).not.toBe(dogs[2]);

    popCurrentContainer();
  });

  it("recursively resolves dependencies in single scope", () => {
    const container = createContainerSingle();
    const house = container.resolve(IHouse);

    expect(house.houseCity).toBe("Kladno");
    expect(house.dog.dogName).toBe("Jamie");
    expect(house.cat.catName).toBe("Garfield");
    expect(house.fish.fishName).toBe("Nemo");
    expect(house.cat.garden.size).toBe(150);
    expect(house.fish.garden.size).toBe(150);
    expect(house.fish.garden).toBe(house.cat.garden);
  });

  it("recursively resolves dependencies in transient scope", () => {
    const container = createContainerTransient();
    const house = container.resolve(IHouse);

    expect(house.houseCity).toBe("Kladno");
    expect(house.dog.dogName).toBe("Jamie");
    expect(house.cat.catName).toBe("Garfield");
    expect(house.fish.fishName).toBe("Nemo");
    expect(house.cat.garden.size).toBe(150);
    expect(house.fish.garden.size).toBe(150);
    expect(house.fish.garden).not.toBe(house.cat.garden);
  });

  it("resolves can resolve all instances for all registrations of one symbol", () => {
    const container = new Container({ defaultLifetime: ILifetime.Single });
    container.registerClass(IPluginConsumer, PluginConsumer);
    container.register(IPluginEnumeration, undefined, (cont) => cont.resolveAll(IPlugin));
    container.registerClass(IPlugin, PluginA).transientInstance();
    container.registerClass(IPlugin, PluginB).transientInstance();
    container.registerClass(IPlugin, PluginC).transientInstance();

    const pc = container.resolve(IPluginConsumer);

    expect(pc.plugins[0].k).toBe(0);
    expect(pc.plugins[1].k).toBe(1);
    expect(pc.plugins[2].k).toBe(2);
  });

  it("resolves can resolve all instances for all registrations of one symbol - allows symbol redirect", () => {
    const container = new Container({ defaultLifetime: ILifetime.Single });
    container.registerClass(IPluginConsumer, PluginConsumer);
    container.register(IPluginEnumeration, undefined, (cont) => cont.resolveAll(IPlugin));
    container.register(IPlugin, IPluginA).transientInstance();
    container.register(IPlugin, IPluginB).transientInstance();
    container.register(IPlugin, IPluginC).transientInstance();

    container.registerClass(IPluginA, PluginA);
    container.registerClass(IPluginB, PluginB);
    container.registerClass(IPluginC, PluginC);

    const pc = container.resolve(IPluginConsumer);

    expect(pc.plugins[0].k).toBe(0);
    expect(pc.plugins[1].k).toBe(1);
    expect(pc.plugins[2].k).toBe(2);
  });

  it("allows to inject lazy object creators", () => {
    const container = new Container({ defaultLifetime: ILifetime.Single });
    const callOrder: any[] = [];

    container
      .register(IA, () => {
        callOrder.push("CA");
        return new A();
      })
      .onActivated(() => {
        callOrder.push("AA");
      });
    container
      .register(IB, () => {
        callOrder.push("CB");
        return new B();
      })
      .onActivated(() => {
        callOrder.push("AB");
      });
    container
      .register(IC, () => {
        callOrder.push("CC");
        return new C();
      })
      .onActivated(() => {
        callOrder.push("AC");
      });

    container
      .register(IX, () => {
        callOrder.push("CX");
        return new X();
      })
      .onActivated(() => {
        callOrder.push("AX");
      });
    const x = container.resolve(IX);

    expect(callOrder).toEqual(["CX", "AX"]);
    x.a();
    expect(callOrder).toEqual(["CX", "AX", "CA", "CB", "CC", "AB", "AC", "AA"]);
    x.a();
    expect(callOrder).toEqual(["CX", "AX", "CA", "CB", "CC", "AB", "AC", "AA"]);
  });
});

describe("Container lifecycle event", () => {
  it("runs onPreparing before resolving the instance", () => {
    const container = new Container({ defaultLifetime: ILifetime.PerDependency });
    const callOrder: any[] = [];

    container
      .register(IA, () => {
        callOrder.push("CA");
        return new A();
      })
      .onPreparing(() => {
        callOrder.push("PA");
      });
    container
      .register(IB, () => {
        callOrder.push("CB");
        return new B();
      })
      .onPreparing(() => {
        callOrder.push("PB");
      });
    container.register(IC, () => new C());

    container.resolve(IA);

    expect(callOrder).toEqual(["PA", "CA", "PB", "CB"]);
  });

  it("runs onActivating right after the instance has been resolved", () => {
    const container = new Container({ defaultLifetime: ILifetime.PerDependency });
    const callOrder: any[] = [];

    container
      .register(IA, () => {
        callOrder.push("CA");
        return new A();
      })
      .onActivating(() => {
        callOrder.push("AA");
      });
    container
      .register(IB, () => {
        callOrder.push("CB");
        return new B();
      })
      .onActivating(() => {
        callOrder.push("AB");
      });
    container
      .register(IC, () => {
        callOrder.push("CC");
        return new C();
      })
      .onActivating(() => {
        callOrder.push("AC");
      });

    container.resolve(IA);

    expect(callOrder).toEqual(["CA", "CB", "AB", "CC", "AC", "AA"]);
  });

  it("runs onActivated after the whole resolution flow has finished.", () => {
    const container = new Container({ defaultLifetime: ILifetime.PerDependency });
    const callOrder: any[] = [];

    container
      .register(IA, () => {
        callOrder.push("CA");
        return new A();
      })
      .onActivated(() => {
        callOrder.push("AA");
      });
    container
      .register(IB, () => {
        callOrder.push("CB");
        return new B();
      })
      .onActivated(() => {
        callOrder.push("AB");
      });
    container
      .register(IC, () => {
        callOrder.push("CC");
        return new C();
      })
      .onActivated(() => {
        callOrder.push("AC");
      });

    container.resolve(IA);

    expect(callOrder).toEqual(["CA", "CB", "CC", "AB", "AC", "AA"]);
  });
});

describe("Creator stack", () => {
  it("keeps track of creators to be used for current dependency chain", () => {
    class A {
      constructor(public b = IB()) {}
    }

    class B {
      constructor(public c = IC()) {}
    }

    class C {
      constructor() {
        expect(getCreatorStack()).toEqual([C, B, A]);
      }
    }

    const IA = TypeSymbol<A>("IA");
    const IB = TypeSymbol<B>("IB");
    const IC = TypeSymbol<C>("IC");

    const cont = new Container({ defaultLifetime: ILifetime.PerLifetimeScope });
    cont.registerClass(IA, A);
    cont.registerClass(IB, B);
    cont.registerClass(IC, C);
    cont.resolve(IA);
  });

  it("allows to supply different implementations based on creator stack content", () => {
    class A {
      constructor(public b = IB(), public c = IC(), public d = ID()) {}
      v = "a";
    }

    class B {
      constructor(public c = IC()) {}
      v = "b";
    }

    class C1 {
      constructor(public c = IC()) {}
      v = "c1";
    }

    class C2 {
      v = "c2";
    }

    class D {
      v = "d";
    }

    const IA = TypeSymbol<A>("IA");
    const IB = TypeSymbol<B>("IB");
    const IC = TypeSymbol<any>("IC");
    const IC1 = TypeSymbol<C1>("IC1");
    const IC2 = TypeSymbol<C2>("IC1");
    const ID = TypeSymbol<D>("ID");

    const cont = new Container({ defaultLifetime: ILifetime.PerLifetimeScope });
    cont.registerClass(IA, A);
    cont.registerClass(IB, B);
    cont
      .register(IC, () => {
        const consumer = getCreatorStack()[1];
        if (consumer === A) return IC1();
        if (consumer === B) return IC2();
        return ID();
      })
      .transientInstance();
    cont.registerClass(IC1, C1);
    cont.registerClass(IC2, C2);
    cont.registerClass(ID, D);
    const a = cont.resolve(IA);

    expect(a.v).toBe("a");
    expect(a.b.v).toBe("b");
    expect(a.b.c.v).toBe("c2");
    expect(a.c.v).toBe("c1");
    expect(a.c.c.v).toBe("d");
    expect(a.d.v).toBe("d");
  });
});
