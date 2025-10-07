import { useState, type ReactNode } from 'react';
import type { Unit } from '../types/units';
import { UnitContext } from './UnitContextValue';

interface UnitProviderProps {
  children: ReactNode;
}

export function UnitProvider({ children }: UnitProviderProps) {
  const [selectedUnit, setSelectedUnit] = useState<Unit>('т');

  const handleSetSelectedUnit = (unit: Unit) => {
    console.log('UnitContext: Setting unit to', unit);
    setSelectedUnit(unit);
  };

  return (
    <UnitContext.Provider value={{ selectedUnit, setSelectedUnit: handleSetSelectedUnit }}>
      {children}
    </UnitContext.Provider>
  );
}

