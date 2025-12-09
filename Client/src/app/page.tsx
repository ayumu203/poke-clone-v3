import DSLayout from '@/components/templates/DSLayout';
import Header from '@/components/organisms/Header';
import TopScreen from '@/components/organisms/TopScreen';
import BottomScreen from '@/components/organisms/BottomScreen';

export default function Home() {
  return (
    <DSLayout>
      <Header />
      <TopScreen backgroundImage="/Images/art_003.png" />
      <BottomScreen />
    </DSLayout>
  );
}
