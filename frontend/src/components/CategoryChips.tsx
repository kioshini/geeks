import type { ProductType } from '../lib/api';

export function CategoryChips({ types, active, onSelect }: { types: ProductType[]; active: string; onSelect: (name: string) => void }) {
	return (
		<div className="flex flex-wrap gap-2">
			<button
				onClick={() => onSelect('')}
				className={chipCls(!active)}
			>
				Все категории
			</button>
			{types.map(t => (
				<button
					key={t.id}
					onClick={() => onSelect(t.name)}
					className={chipCls(active === t.name)}
				>
					{t.name}
				</button>
			))}
		</div>
	);
}

function chipCls(active: boolean) {
	return `px-3 py-1 rounded-full text-sm font-medium transition-colors ${
		active
			? 'bg-orange-500 text-white'
			: 'bg-gray-100 text-gray-700 hover:bg-gray-200'
	}`
}
