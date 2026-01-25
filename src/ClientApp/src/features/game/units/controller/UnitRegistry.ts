import { UnitController } from "./UnitController";

class UnitRegistry {
  private map = new Map<string, UnitController>();

  /**
   * Связать controller с actorId
   * Вызывается из React-компонента
   */
  bind(id: string, controller: UnitController) {
    this.map.set(id, controller);
  }

  /**
   * Получить контроллер
   */
  get(id: string): UnitController {
    const controller = this.map.get(id);
    if (!controller) throw new Error("Controller not bound: " + id);
    return controller;;
  }

  /**
   * Очистка
   */
  reset() {
    this.map.clear();
  }
}

export const unitRegistry = new UnitRegistry();
