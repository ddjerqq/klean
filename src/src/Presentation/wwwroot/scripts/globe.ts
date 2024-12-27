import ThreeGlobe from "three-globe";
import * as THREE from "three";
import {OrbitControls} from "three/examples/jsm/controls/OrbitControls.js";
import * as countryData from "./country_data.json";
import * as flightData from "./flights.json";
import * as airportData from "./airports.json";

export class GlobeContext {
  private readonly _containerElement: HTMLCanvasElement;
  private readonly _renderer: THREE.WebGLRenderer;
  private readonly _scene: THREE.Scene;
  private readonly _camera: THREE.PerspectiveCamera;
  private readonly _controls: OrbitControls;
  private readonly _globe: ThreeGlobe;

  // getter for client width and height
  get containerDimensions() {
    const {width, height} = this._containerElement.getBoundingClientRect();
    return {width, height}
  }

  public constructor(containerElement: HTMLCanvasElement) {
    this._containerElement = containerElement;

    // init renderer
    this._renderer = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true,
      canvas: containerElement
    });
    this._renderer.setPixelRatio(window.devicePixelRatio);
    this._renderer.setSize(this.containerDimensions.width, this.containerDimensions.height);

    // init scene
    this._scene = new THREE.Scene();
    this._scene.add(new THREE.AmbientLight('#bbbbbb', 0.3));

    // init camera
    this._camera = new THREE.PerspectiveCamera();
    this._camera.aspect = this.containerDimensions.width / this.containerDimensions.height;
    this._camera.updateProjectionMatrix();
    this._camera.position.z = 150;
    this._camera.position.x = 150;
    this._camera.position.y = 0;
    this._camera.fov = 80;

    this._scene.add(this._camera);

    // init lights
    const dLight = new THREE.DirectionalLight('#ffffff', 3);
    const dLight1 = new THREE.DirectionalLight('#7982f6', 3);
    const pLight = new THREE.PointLight('#8566cc', 3);

    dLight.position.set(-800, 2000, 400);
    dLight1.position.set(-200, 500, 200);
    pLight.position.set(-200, 500, 200);

    this._camera.add(dLight);
    this._camera.add(dLight1);
    this._camera.add(pLight);

    // init controls
    this._controls = new OrbitControls(this._camera, this._renderer.domElement);
    this._controls.enableDamping = true;
    this._controls.dampingFactor = 0.01;
    this._controls.enableRotate = true;
    this._controls.enableZoom = false;
    this._controls.enablePan = false;
    this._controls.rotateSpeed = 0.3;
    this._controls.minPolarAngle = Math.PI / 4;
    this._controls.maxPolarAngle = Math.PI / 2;

    this._globe = GlobeContext.createGlobe();
    this._scene.add(this._globe);

    window.addEventListener("resize", () => this.onWindowResize(), false);
  }

  private static createGlobe(): ThreeGlobe {
    const globe = new ThreeGlobe({
      waitForGlobeReady: true,
      animateIn: true,
    })
      .hexPolygonsData(countryData.features)
      .hexPolygonResolution(4)
      .hexPolygonMargin(0.7)
      .showAtmosphere(true)
      .atmosphereColor("#80cfb4")
      .atmosphereAltitude(0.25)
      .hexPolygonColor((e: any) => ["USA", "GEO"].includes(e.properties.ISO_A3) ? "rgba(255,255,255, 1)" : "rgba(255,255,255, 0.5)");

    setTimeout(() => {
      globe.arcsData(flightData.flights)
        .arcColor(() => "#ac36d3")
        .arcAltitude(0.05)
        .arcStroke(0.3)
        .arcDashLength(0.9)
        .arcDashGap(4)
        .arcDashAnimateTime(1000)
        .arcsTransitionDuration(1000)
        .arcDashInitialGap((e: { order: number }) => e.order)
        .labelsData(airportData.airports)
        .labelColor(() => "#ffffff")
        .labelDotOrientation("bottom")
        .labelDotRadius(0.3)
        .labelSize((e: { size: number }) => e.size)
        .labelText("city")
        .labelResolution(6)
        .labelAltitude(0.01)
        .pointsData(airportData.airports)
        .pointColor(() => "#ffffff")
        .pointsMerge(true)
        .pointAltitude(0.07)
        .pointRadius(0.05);
    }, 1000);

    globe.rotateY(-Math.PI * (5 / 9));
    globe.rotateZ(-Math.PI / 6);

    const globeMaterial = globe.globeMaterial();
    // @ts-ignore
    globeMaterial.color = new THREE.Color('#135c27');
    // @ts-ignore
    globeMaterial.emissive = new THREE.Color('#043c29');
    // @ts-ignore
    globeMaterial.emissiveIntensity = 0.2;
    // @ts-ignore
    globeMaterial.shininess = 0.7;
    // @ts-ignore
    globeMaterial.wireframe = false;

    return globe;
  }

  private onWindowResize() {
    this._camera.aspect = this.containerDimensions.width / this.containerDimensions.height;
    this._camera.updateProjectionMatrix();
    this._renderer.setSize(this.containerDimensions.width, this.containerDimensions.height);
  }

  private animate() {
    this._camera.lookAt(this._scene.position);
    this._controls.update();
    this._renderer.render(this._scene, this._camera);
    this._globe.rotation.y -= 0.00025;
    requestAnimationFrame(() => this.animate());
  }

  private static randomSpherePoint() {
    const radius = Math.random() * 25 + 25;
    const u = Math.random();
    const v = Math.random();
    const theta = 2 * Math.PI * u;
    const phi = Math.acos(2 * v - 1);
    let x = 20 * radius * Math.sin(phi) * Math.cos(theta);
    let y = 20 * radius * Math.sin(phi) * Math.sin(theta);
    let z = 20 * radius * Math.cos(phi);

    return {
      pos: new THREE.Vector3(x, y, z),
      hue: 0.6, // radius * 0.02 + 0.5
      minDist: radius,
    };
  }

  private getStarField() {
    const verts = [];
    const colors = [];
    const positions = [];
    let col;
    for (let i = 0; i < 500; i += 1) {
      let p = GlobeContext.randomSpherePoint();
      const {pos, hue} = p;
      positions.push(p);
      col = new THREE.Color().setHSL(hue, 0.2, Math.random());
      verts.push(pos.x, pos.y, pos.z);
      colors.push(col.r, col.g, col.b);
    }
    const geo = new THREE.BufferGeometry();
    geo.setAttribute("position", new THREE.Float32BufferAttribute(verts, 3));
    geo.setAttribute("color", new THREE.Float32BufferAttribute(colors, 3));
    const mat = new THREE.PointsMaterial({
      size: 0.2,
      vertexColors: true,
      fog: false,
    });

    return new THREE.Points(geo, mat);
  }

  public start() {
    this.onWindowResize();
    this.animate();
  }
}

const el = document.querySelector("#globe")
const globe = new GlobeContext(el as HTMLCanvasElement);
globe.start();
