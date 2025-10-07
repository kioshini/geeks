import { useContext } from 'react';
import { UnitContext } from '../contexts/UnitContextValue';

export const useUnit = () => {
  const context = useContext(UnitContext);
  if (context === undefined) {
    throw new Error('useUnit must be used within a UnitProvider');
  }
  return context;
};
