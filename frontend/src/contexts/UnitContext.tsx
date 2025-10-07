import { createContext, useContext, useState, type ReactNode } from 'react';

export type Unit = 'шт' | 'м' | 'т';

interface UnitContextType {
  selectedUnit: Unit;
  setSelectedUnit: (unit: Unit) => void;
}

const UnitContext = createContext<UnitContextType | undefined>(undefined);

interface UnitProviderProps {
  children: ReactNode;
}

export function UnitProvider({ children }: UnitProviderProps) {
  const [selectedUnit, setSelectedUnit] = useState<Unit>('шт');

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

export function useUnit() {
  const context = useContext(UnitContext);
  if (context === undefined) {
    throw new Error('useUnit must be used within a UnitProvider');
  }
  return context;
}
